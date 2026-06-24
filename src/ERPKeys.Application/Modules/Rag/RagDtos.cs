namespace ERPKeys.Application.Modules.Rag;

public record RagDocumentDto(
    Guid DocumentId,
    string Name,
    string SourceType,
    int ChunkCount,
    DateTime UploadedAt,
    string? RequiredPermission = null);

public record RagSourceDto(
    Guid DocumentId,
    string DocumentName,
    int ChunkIndex,
    string Excerpt,
    double Relevance);

public record RagAnswerDto(
    string Answer,
    IReadOnlyList<RagSourceDto> Sources);

public record RagConversationTurnDto(
    string Role,
    string Text);

public record RagSearchHit(
    Guid DocumentId,
    string DocumentName,
    int ChunkIndex,
    string Content,
    double Distance);

public record RagChunkEmbedding(
    int ChunkIndex,
    string Content,
    float[] Embedding);
