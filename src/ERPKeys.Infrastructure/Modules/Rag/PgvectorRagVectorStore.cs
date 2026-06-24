using ERPKeys.Application.Modules.Rag;
using ERPKeys.Domain.Modules.Rag;
using ERPKeys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace ERPKeys.Infrastructure.Modules.Rag;

public sealed class PgvectorRagVectorStore : IRagVectorStore
{
    private readonly AppDbContext _db;

    public PgvectorRagVectorStore(AppDbContext db) => _db = db;

    public async Task SaveDocumentAsync(
        Guid organizationId,
        Guid documentId,
        string documentName,
        string sourceType,
        string contentHash,
        string? requiredPermission,
        Guid? uploadedByUserId,
        IReadOnlyList<RagChunkEmbedding> chunks,
        CancellationToken ct = default)
    {
        foreach (var item in chunks)
        {
            var chunk = new DocumentChunk(
                organizationId,
                documentId,
                documentName,
                sourceType,
                contentHash,
                requiredPermission,
                uploadedByUserId,
                item.ChunkIndex,
                item.Content);
            _db.DocumentChunks.Add(chunk);
            _db.Entry(chunk).Property<Vector>("Embedding").CurrentValue =
                new Vector(item.Embedding);
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<RagSearchHit>> SearchAsync(
        Guid organizationId,
        float[] queryEmbedding,
        IReadOnlyCollection<string> allowedPermissions,
        IReadOnlyCollection<Guid> allowedDocumentIds,
        int limit,
        CancellationToken ct = default)
    {
        var queryVector = new Vector(queryEmbedding);
        var take = Math.Clamp(limit, 1, 12);
        var permissions = allowedPermissions.ToArray();
        var documentIds = allowedDocumentIds.ToArray();

        if (documentIds.Length == 0)
            return [];

        var hits = await _db.DocumentChunks
            .AsNoTracking()
            .Where(chunk =>
                chunk.OrganizationId == organizationId &&
                !chunk.IsDeleted &&
                documentIds.Contains(chunk.DocumentId) &&
                (chunk.RequiredPermission == null || permissions.Contains(chunk.RequiredPermission)))
            .OrderBy(chunk => EF.Property<Vector>(chunk, "Embedding").CosineDistance(queryVector))
            .Take(take)
            .Select(chunk => new
            {
                chunk.DocumentId,
                chunk.DocumentName,
                chunk.ChunkIndex,
                chunk.Content,
                Distance = EF.Property<Vector>(chunk, "Embedding").CosineDistance(queryVector)
            })
            .ToListAsync(ct);

        return hits
            .Select(hit => new RagSearchHit(
                hit.DocumentId,
                hit.DocumentName,
                hit.ChunkIndex,
                hit.Content,
                hit.Distance))
            .ToList();
    }
}
