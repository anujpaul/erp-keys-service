using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.Rag;

public class DocumentChunk : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid DocumentId { get; private set; }
    public string DocumentName { get; private set; } = string.Empty;
    public string SourceType { get; private set; } = string.Empty;
    public string ContentHash { get; private set; } = string.Empty;
    public string? RequiredPermission { get; private set; }
    public Guid? UploadedByUserId { get; private set; }
    public int ChunkIndex { get; private set; }
    public string Content { get; private set; } = string.Empty;

    private DocumentChunk() { }

    public DocumentChunk(
        Guid organizationId,
        Guid documentId,
        string documentName,
        string sourceType,
        string contentHash,
        string? requiredPermission,
        Guid? uploadedByUserId,
        int chunkIndex,
        string content)
    {
        OrganizationId = organizationId;
        DocumentId = documentId;
        DocumentName = documentName.Trim();
        SourceType = sourceType.Trim().ToLowerInvariant();
        ContentHash = contentHash.Trim();
        RequiredPermission = string.IsNullOrWhiteSpace(requiredPermission)
            ? null
            : requiredPermission.Trim().ToLowerInvariant();
        UploadedByUserId = uploadedByUserId;
        ChunkIndex = chunkIndex;
        Content = content.Trim();
    }
}
