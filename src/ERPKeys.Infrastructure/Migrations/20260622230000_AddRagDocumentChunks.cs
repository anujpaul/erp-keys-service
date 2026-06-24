using ERPKeys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260622230000_AddRagDocumentChunks")]
public partial class AddRagDocumentChunks : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS vector;");

        migrationBuilder.CreateTable(
            name: "document_chunks",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                document_id = table.Column<Guid>(type: "uuid", nullable: false),
                document_name = table.Column<string>(
                    type: "character varying(300)",
                    maxLength: 300,
                    nullable: false),
                source_type = table.Column<string>(
                    type: "character varying(30)",
                    maxLength: 30,
                    nullable: false),
                chunk_index = table.Column<int>(type: "integer", nullable: false),
                content = table.Column<string>(type: "text", nullable: false),
                embedding = table.Column<Vector>(type: "vector(1536)", nullable: false),
                created_at = table.Column<DateTime>(
                    type: "timestamp without time zone",
                    nullable: false),
                updated_at = table.Column<DateTime>(
                    type: "timestamp without time zone",
                    nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_document_chunks", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_document_chunks_organization_id_document_id",
            table: "document_chunks",
            columns: new[] { "organization_id", "document_id" });

        migrationBuilder.CreateIndex(
            name: "ix_document_chunks_organization_id_document_id_chunk_index",
            table: "document_chunks",
            columns: new[] { "organization_id", "document_id", "chunk_index" },
            unique: true);

        migrationBuilder.Sql(
            """
            CREATE INDEX ix_document_chunks_embedding_hnsw
            ON document_chunks
            USING hnsw (embedding vector_cosine_ops);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "document_chunks");
    }
}
