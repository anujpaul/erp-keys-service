using ERPKeys.Domain.Modules.Rag;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector;

namespace ERPKeys.Infrastructure.Persistence.Configurations;

public sealed class DocumentChunkConfiguration : IEntityTypeConfiguration<DocumentChunk>
{
    public void Configure(EntityTypeBuilder<DocumentChunk> b)
    {
        b.ToTable("document_chunks");
        b.HasKey(chunk => chunk.Id);
        b.Property(chunk => chunk.OrganizationId).IsRequired();
        b.Property(chunk => chunk.DocumentId).IsRequired();
        b.Property(chunk => chunk.DocumentName).HasMaxLength(300).IsRequired();
        b.Property(chunk => chunk.SourceType).HasMaxLength(30).IsRequired();
        b.Property(chunk => chunk.ContentHash).HasMaxLength(64).IsRequired();
        b.Property(chunk => chunk.RequiredPermission).HasMaxLength(100);
        b.Property(chunk => chunk.UploadedByUserId);
        b.Property(chunk => chunk.ChunkIndex).IsRequired();
        b.Property(chunk => chunk.Content).IsRequired();
        b.Property<Vector>("Embedding")
            .HasColumnName("embedding")
            .HasColumnType("vector(1536)")
            .IsRequired();
        b.HasIndex(chunk => new
        {
            chunk.OrganizationId,
            chunk.DocumentId,
            chunk.ChunkIndex
        }).IsUnique();
        b.HasIndex(chunk => new { chunk.OrganizationId, chunk.DocumentId });
        b.HasIndex(chunk => new { chunk.OrganizationId, chunk.ContentHash });
        b.HasIndex(chunk => new { chunk.OrganizationId, chunk.RequiredPermission });
        b.HasIndex(chunk => new { chunk.OrganizationId, chunk.UploadedByUserId });
        b.HasIndex("Embedding")
            .HasDatabaseName("ix_document_chunks_embedding_hnsw")
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");
    }
}
