using ERPKeys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260623235600_GrantKnowledgeAccessToReadRoles")]
    public partial class GrantKnowledgeAccessToReadRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO role_permissions (id, role_id, module, action, created_at, updated_at, is_deleted)
                SELECT
                    md5(random()::text || clock_timestamp()::text || r.id::text)::uuid,
                    r.id,
                    'Knowledge',
                    'Read',
                    now(),
                    now(),
                    false
                FROM roles r
                WHERE r.name IN ('Admin', 'Manager', 'Viewer', 'AppSupport')
                  AND NOT r.is_deleted
                  AND NOT EXISTS (
                      SELECT 1
                      FROM role_permissions rp
                      WHERE rp.role_id = r.id
                        AND rp.module = 'Knowledge'
                        AND rp.action = 'Read'
                        AND NOT rp.is_deleted
                  );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM role_permissions rp
                USING roles r
                WHERE rp.role_id = r.id
                  AND r.name IN ('Admin', 'Manager', 'Viewer', 'AppSupport')
                  AND rp.module = 'Knowledge'
                  AND rp.action = 'Read';
                """);
        }
    }
}
