namespace ERPKeys.Application.Modules.Rag;

public interface IRagService
{
    Task<RagDocumentDto> IngestAsync(
        string documentName,
        string sourceType,
        string content,
        string? requiredPermission = null,
        CancellationToken ct = default);

    Task<IReadOnlyList<RagDocumentDto>> GetDocumentsAsync(CancellationToken ct = default);

    Task DeleteDocumentAsync(Guid documentId, CancellationToken ct = default);

    Task<RagAnswerDto> AskAsync(
        string question,
        IReadOnlyList<RagConversationTurnDto>? history = null,
        CancellationToken ct = default);
}

public interface IRagVectorStore
{
    Task SaveDocumentAsync(
        Guid organizationId,
        Guid documentId,
        string documentName,
        string sourceType,
        string contentHash,
        string? requiredPermission,
        Guid? uploadedByUserId,
        IReadOnlyList<RagChunkEmbedding> chunks,
        CancellationToken ct = default);

    Task<IReadOnlyList<RagSearchHit>> SearchAsync(
        Guid organizationId,
        float[] queryEmbedding,
        IReadOnlyCollection<string> allowedPermissions,
        IReadOnlyCollection<Guid> allowedDocumentIds,
        int limit,
        CancellationToken ct = default);
}

public interface IOpenAiRagClient
{
    Task<IReadOnlyList<float[]>> CreateEmbeddingsAsync(
        IReadOnlyList<string> inputs,
        CancellationToken ct = default);

    Task<string> CreateGroundedAnswerAsync(
        string question,
        IReadOnlyList<RagSearchHit> context,
        IReadOnlyList<RagConversationTurnDto>? history = null,
        CancellationToken ct = default);
}
