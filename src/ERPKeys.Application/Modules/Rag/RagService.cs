using System.Security.Cryptography;
using System.Text;
using ERPKeys.Application.Common.Interfaces;
using ERPKeys.Application.Common.Security;
using Microsoft.EntityFrameworkCore;

namespace ERPKeys.Application.Modules.Rag;

public sealed class RagService : IRagService
{
    private const int ChunkSize = 1_200;
    private const int ChunkOverlap = 180;
    private const int SearchLimit = 6;
    private const int MaxDocumentCharacters = 300_000;
    private const int MaxDocumentChunks = 300;

    private readonly IAppDbContext _db;
    private readonly ICurrentOrganizationService _org;
    private readonly ICurrentUserService _user;
    private readonly IOpenAiRagClient _openAi;
    private readonly IRagVectorStore _vectorStore;

    public RagService(
        IAppDbContext db,
        ICurrentOrganizationService org,
        ICurrentUserService user,
        IOpenAiRagClient openAi,
        IRagVectorStore vectorStore)
    {
        _db = db;
        _org = org;
        _user = user;
        _openAi = openAi;
        _vectorStore = vectorStore;
    }

    public async Task<RagDocumentDto> IngestAsync(
        string documentName,
        string sourceType,
        string content,
        string? requiredPermission = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(documentName))
            throw new InvalidOperationException("Document name is required.");
        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException("The document contains no readable text.");
        if (content.Length > MaxDocumentCharacters)
            throw new InvalidOperationException(
                "The document is too large for interactive indexing. Keep extracted text under 300,000 characters.");

        requiredPermission = NormalizeRequiredPermission(requiredPermission);
        var contentHash = ComputeContentHash(content);
        var existing = await _db.DocumentChunks
            .AsNoTracking()
            .Where(chunk =>
                chunk.OrganizationId == _org.OrganizationId &&
                chunk.ContentHash == contentHash &&
                !chunk.IsDeleted)
            .GroupBy(chunk => new
            {
                chunk.DocumentId,
                chunk.DocumentName,
                chunk.SourceType,
                chunk.RequiredPermission
            })
            .Select(group => new RagDocumentDto(
                group.Key.DocumentId,
                group.Key.DocumentName,
                group.Key.SourceType,
                group.Count(),
                group.Min(chunk => chunk.CreatedAt),
                group.Key.RequiredPermission))
            .FirstOrDefaultAsync(ct);
        if (existing is not null)
            return existing;

        var chunks = SplitIntoChunks(content);
        if (chunks.Count == 0)
            throw new InvalidOperationException("The document contains no readable text.");
        if (chunks.Count > MaxDocumentChunks)
            throw new InvalidOperationException(
                "The document produced too many chunks. Split it into smaller documents and try again.");

        var legacyExisting = await FindExistingDocumentByChunksAsync(
            documentName,
            sourceType,
            chunks,
            ct);
        if (legacyExisting is not null)
            return legacyExisting;

        var embeddings = await _openAi.CreateEmbeddingsAsync(chunks, ct);
        if (embeddings.Count != chunks.Count)
            throw new InvalidOperationException("The embedding provider returned an incomplete result.");

        var documentId = Guid.NewGuid();
        var chunkEmbeddings = chunks
            .Select((chunk, index) => new RagChunkEmbedding(index, chunk, embeddings[index]))
            .ToList();

        await _vectorStore.SaveDocumentAsync(
            _org.OrganizationId,
            documentId,
            documentName,
            sourceType,
            contentHash,
            requiredPermission,
            _user.UserId,
            chunkEmbeddings,
            ct);

        return new RagDocumentDto(
            documentId,
            documentName.Trim(),
            sourceType.Trim().ToLowerInvariant(),
            chunks.Count,
            DateTime.UtcNow,
            requiredPermission);
    }

    public async Task<IReadOnlyList<RagDocumentDto>> GetDocumentsAsync(
        CancellationToken ct = default)
    {
        return await GetVisibleDocumentsQuery()
            .ToListAsync(ct);
    }

    public async Task DeleteDocumentAsync(Guid documentId, CancellationToken ct = default)
    {
        var allowedPermissions = _user.Permissions.ToArray();
        var isAdmin = _user.IsAdmin;
        var currentUserId = _user.UserId;
        var chunks = await _db.DocumentChunks
            .Where(chunk =>
                chunk.OrganizationId == _org.OrganizationId &&
                chunk.DocumentId == documentId &&
                !chunk.IsDeleted &&
                (isAdmin || chunk.UploadedByUserId == currentUserId) &&
                (chunk.RequiredPermission == null || allowedPermissions.Contains(chunk.RequiredPermission)))
            .ToListAsync(ct);

        if (chunks.Count == 0)
            throw new InvalidOperationException("Document not found.");

        foreach (var chunk in chunks)
            chunk.SoftDelete();
        await _db.SaveChangesAsync(ct);
    }

    public async Task<RagAnswerDto> AskAsync(
        string question,
        IReadOnlyList<RagConversationTurnDto>? history = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(question) || question.Trim().Length < 3)
            throw new InvalidOperationException("Enter a question with at least 3 characters.");

        var documents = await GetSearchableDocumentsQuery().ToListAsync(ct);
        if (documents.Count == 0)
        {
            return new RagAnswerDto(
                "Upload at least one knowledge document before asking questions.",
                []);
        }

        var allowedPermissions = _user.Permissions.ToArray();
        var allowedDocumentIds = documents.Select(document => document.DocumentId).ToArray();
        var retrievalQuestion = BuildRetrievalQuestion(question.Trim(), history);
        var embedding = (await _openAi.CreateEmbeddingsAsync([retrievalQuestion], ct)).Single();
        var hits = await _vectorStore.SearchAsync(
            _org.OrganizationId,
            embedding,
            allowedPermissions,
            allowedDocumentIds,
            SearchLimit,
            ct);

        if (hits.Count == 0)
        {
            return new RagAnswerDto(
                "I could not find relevant information in the uploaded knowledge base.",
                []);
        }

        var answer = await _openAi.CreateGroundedAnswerAsync(question.Trim(), hits, history, ct);
        var sources = hits.Select(hit => new RagSourceDto(
            hit.DocumentId,
            hit.DocumentName,
            hit.ChunkIndex,
            CreateExcerpt(hit.Content),
            Math.Clamp(1d - hit.Distance, 0d, 1d)))
            .ToList();

        return new RagAnswerDto(answer, sources);
    }

    private IQueryable<RagDocumentDto> GetVisibleDocumentsQuery()
    {
        var isAdmin = _user.IsAdmin;
        var currentUserId = _user.UserId;
        return GetSearchableDocumentChunksQuery()
            .Where(chunk => isAdmin || chunk.UploadedByUserId == currentUserId)
            .GroupBy(chunk => new
            {
                chunk.DocumentId,
                chunk.DocumentName,
                chunk.SourceType,
                chunk.RequiredPermission
            })
            .OrderByDescending(group => group.Min(chunk => chunk.CreatedAt))
            .Select(group => new RagDocumentDto(
                group.Key.DocumentId,
                group.Key.DocumentName,
                group.Key.SourceType,
                group.Count(),
                group.Min(chunk => chunk.CreatedAt),
                group.Key.RequiredPermission));
    }

    private IQueryable<RagDocumentDto> GetSearchableDocumentsQuery()
    {
        return GetSearchableDocumentChunksQuery()
            .GroupBy(chunk => new
            {
                chunk.DocumentId,
                chunk.DocumentName,
                chunk.SourceType,
                chunk.RequiredPermission
            })
            .OrderByDescending(group => group.Min(chunk => chunk.CreatedAt))
            .Select(group => new RagDocumentDto(
                group.Key.DocumentId,
                group.Key.DocumentName,
                group.Key.SourceType,
                group.Count(),
                group.Min(chunk => chunk.CreatedAt),
                group.Key.RequiredPermission));
    }

    private IQueryable<ERPKeys.Domain.Modules.Rag.DocumentChunk> GetSearchableDocumentChunksQuery()
    {
        var allowedPermissions = _user.Permissions.ToArray();
        return _db.DocumentChunks
            .AsNoTracking()
            .Where(chunk =>
                chunk.OrganizationId == _org.OrganizationId &&
                !chunk.IsDeleted)
            .Where(chunk =>
                chunk.RequiredPermission == null ||
                allowedPermissions.Contains(chunk.RequiredPermission));
    }

    private static string BuildRetrievalQuestion(
        string question,
        IReadOnlyList<RagConversationTurnDto>? history)
    {
        var recentHistory = NormalizeHistory(history).TakeLast(4).ToList();
        if (recentHistory.Count == 0)
            return question;

        var builder = new StringBuilder();
        builder.AppendLine("Recent conversation:");
        foreach (var turn in recentHistory)
            builder.AppendLine($"{turn.Role}: {Truncate(turn.Text, 900)}");
        builder.AppendLine();
        builder.AppendLine("Current question:");
        builder.AppendLine(question);
        return builder.ToString();
    }

    public static IReadOnlyList<RagConversationTurnDto> NormalizeHistory(
        IReadOnlyList<RagConversationTurnDto>? history)
    {
        if (history is null || history.Count == 0)
            return [];

        return history
            .Where(turn =>
                (turn.Role.Equals("user", StringComparison.OrdinalIgnoreCase) ||
                 turn.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase)) &&
                !string.IsNullOrWhiteSpace(turn.Text))
            .TakeLast(8)
            .Select(turn => new RagConversationTurnDto(
                turn.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase) ? "assistant" : "user",
                Truncate(turn.Text.Trim(), 1_500)))
            .ToList();
    }

    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..maxLength] + "...";

    private static string ComputeContentHash(string content)
    {
        var normalized = content.Replace("\r\n", "\n").Replace('\r', '\n').Trim();
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalized)))
            .ToLowerInvariant();
    }

    private async Task<RagDocumentDto?> FindExistingDocumentByChunksAsync(
        string documentName,
        string sourceType,
        IReadOnlyList<string> chunks,
        CancellationToken ct)
    {
        var normalizedName = documentName.Trim();
        var normalizedSourceType = sourceType.Trim().ToLowerInvariant();

        var candidates = await _db.DocumentChunks
            .AsNoTracking()
            .Where(chunk =>
                chunk.OrganizationId == _org.OrganizationId &&
                chunk.DocumentName == normalizedName &&
                chunk.SourceType == normalizedSourceType &&
                !chunk.IsDeleted)
            .OrderBy(chunk => chunk.DocumentId)
            .ThenBy(chunk => chunk.ChunkIndex)
            .Select(chunk => new
            {
                chunk.DocumentId,
                chunk.DocumentName,
                chunk.SourceType,
                chunk.RequiredPermission,
                chunk.ChunkIndex,
                chunk.Content,
                chunk.CreatedAt
            })
            .ToListAsync(ct);

        foreach (var documentGroup in candidates.GroupBy(chunk => new
                 {
                     chunk.DocumentId,
                     chunk.DocumentName,
                     chunk.SourceType,
                     chunk.RequiredPermission
                 }))
        {
            var existingChunks = documentGroup
                .OrderBy(chunk => chunk.ChunkIndex)
                .Select(chunk => chunk.Content.Trim())
                .ToList();

            if (existingChunks.Count == chunks.Count &&
                existingChunks.SequenceEqual(chunks.Select(chunk => chunk.Trim())))
            {
                return new RagDocumentDto(
                    documentGroup.Key.DocumentId,
                    documentGroup.Key.DocumentName,
                    documentGroup.Key.SourceType,
                    existingChunks.Count,
                    documentGroup.Min(chunk => chunk.CreatedAt),
                    documentGroup.Key.RequiredPermission);
            }
        }

        return null;
    }

    private static string? NormalizeRequiredPermission(string? requiredPermission)
    {
        if (string.IsNullOrWhiteSpace(requiredPermission))
            return null;

        var value = requiredPermission.Trim().ToLowerInvariant();
        if (!PermissionCatalog.PolicyKeys.Contains(value, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("The selected knowledge access scope is invalid.");

        return value == PermissionKeys.KnowledgeAccess ? null : value;
    }

    private static List<string> SplitIntoChunks(string content)
    {
        var normalized = content.Replace("\r\n", "\n").Replace('\r', '\n').Trim();
        var paragraphs = normalized
            .Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var chunks = new List<string>();
        var current = new StringBuilder();

        foreach (var paragraph in paragraphs)
        {
            if (paragraph.Length > ChunkSize)
            {
                Flush(current, chunks);
                SplitLongText(paragraph, chunks);
                continue;
            }

            if (current.Length > 0 && current.Length + paragraph.Length + 2 > ChunkSize)
            {
                var overlap = Tail(current.ToString(), ChunkOverlap);
                Flush(current, chunks);
                current.Append(overlap);
                if (current.Length > 0)
                    current.AppendLine().AppendLine();
            }

            current.Append(paragraph).AppendLine().AppendLine();
        }

        Flush(current, chunks);
        return chunks.Where(chunk => chunk.Length >= 20).ToList();
    }

    private static void SplitLongText(string text, ICollection<string> chunks)
    {
        var start = 0;
        while (start < text.Length)
        {
            var length = Math.Min(ChunkSize, text.Length - start);
            chunks.Add(text.Substring(start, length).Trim());
            if (start + length >= text.Length)
                break;
            start += Math.Max(1, length - ChunkOverlap);
        }
    }

    private static void Flush(StringBuilder current, ICollection<string> chunks)
    {
        var value = current.ToString().Trim();
        if (value.Length > 0)
            chunks.Add(value);
        current.Clear();
    }

    private static string Tail(string value, int length) =>
        value.Length <= length ? value : value[^length..];

    private static string CreateExcerpt(string content)
    {
        var singleLine = string.Join(' ', content.Split(
            ['\r', '\n'],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        return singleLine.Length <= 240 ? singleLine : singleLine[..237] + "...";
    }
}
