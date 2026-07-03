using ERPKeys.Domain.Modules.SystemAdmin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPKeys.Infrastructure.Persistence.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> b)
    {
        b.ToTable("app_users");
        b.HasKey(e => e.Id);
        b.Property(e => e.OrganizationId).IsRequired();
        b.Property(e => e.PreferredOrganizationId);
        b.HasOne<ERPKeys.Domain.Modules.Organization.Organization>()
            .WithMany()
            .HasForeignKey(e => e.PreferredOrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
        b.Property(e => e.Username).HasMaxLength(100).IsRequired();
        b.Property(e => e.Email).HasMaxLength(200).IsRequired();
        b.Property(e => e.FullName).HasMaxLength(200).IsRequired();
        b.Property(e => e.EmployeeId).HasMaxLength(100);
        b.Property(e => e.JobTitle).HasMaxLength(150);
        b.Property(e => e.Department).HasMaxLength(150);
        b.Property(e => e.Phone).HasMaxLength(50);
        b.Property(e => e.Timezone).HasMaxLength(100);
        b.Property(e => e.Locale).HasMaxLength(20);
        b.Property(e => e.AddressLine1).HasMaxLength(250);
        b.Property(e => e.AddressLine2).HasMaxLength(250);
        b.Property(e => e.City).HasMaxLength(150);
        b.Property(e => e.State).HasMaxLength(150);
        b.Property(e => e.PostalCode).HasMaxLength(30);
        b.Property(e => e.Country).HasMaxLength(2);
        b.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
        b.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        b.Property(e => e.RefreshToken).HasMaxLength(500);
        b.HasIndex(e => new { e.OrganizationId, e.Username }).IsUnique();
        b.HasIndex(e => new { e.OrganizationId, e.EmployeeId })
            .IsUnique()
            .HasFilter("employee_id IS NOT NULL");
        b.HasMany(e => e.UserRoles).WithOne()
            .HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("roles");
        b.HasKey(e => e.Id);
        b.Property(e => e.OrganizationId).IsRequired();
        b.Property(e => e.Name).HasMaxLength(100).IsRequired();
        b.Property(e => e.Description).HasMaxLength(500);
        b.HasIndex(e => new { e.OrganizationId, e.Name }).IsUnique();
        b.HasMany(e => e.Permissions).WithOne()
            .HasForeignKey(p => p.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> b)
    {
        b.ToTable("role_permissions");
        b.HasKey(e => e.Id);
        b.Property(e => e.Module).HasMaxLength(50).IsRequired();
        b.Property(e => e.Action).HasMaxLength(50).IsRequired();
        b.HasIndex(e => new { e.RoleId, e.Module, e.Action }).IsUnique();
    }
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> b)
    {
        b.ToTable("user_roles");
        b.HasKey(e => e.Id);
        b.HasOne(e => e.Role).WithMany()
            .HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
    }
}

public class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> b)
    {
        b.ToTable("audit_logs");
        b.HasKey(e => e.Id);
        b.Property(e => e.OrganizationId).IsRequired();
        b.Property(e => e.Username).HasMaxLength(100);
        b.Property(e => e.Module).HasMaxLength(50);
        b.Property(e => e.Action).HasMaxLength(100);
        b.Property(e => e.EntityId).HasMaxLength(100);
        b.Property(e => e.EntityType).HasMaxLength(100);
        b.Property(e => e.OldValues).HasMaxLength(4000);
        b.Property(e => e.NewValues).HasMaxLength(4000);
        b.Property(e => e.IpAddress).HasMaxLength(50);
        b.HasIndex(e => new { e.OrganizationId, e.OccurredAt });
        b.HasIndex(e => e.UserId);
    }
}

public class IntegrationConfigurationEntityConfiguration
    : IEntityTypeConfiguration<IntegrationConfiguration>
{
    public void Configure(EntityTypeBuilder<IntegrationConfiguration> b)
    {
        b.ToTable("integration_configurations");
        b.HasKey(e => e.Id);
        b.Property(e => e.OrganizationId).IsRequired();
        b.Property(e => e.Code).HasMaxLength(50).IsRequired();
        b.Property(e => e.Name).HasMaxLength(150).IsRequired();
        b.Property(e => e.ServiceCategory).HasMaxLength(100).IsRequired();
        b.Property(e => e.ConnectorType).HasMaxLength(100).IsRequired();
        b.Property(e => e.Description).HasMaxLength(500);
        b.Property(e => e.FieldDefinitionsJson).HasColumnType("jsonb").IsRequired();
        b.Property(e => e.SettingsJson).HasColumnType("jsonb").IsRequired();
        b.Property(e => e.EncryptedSecrets).HasColumnType("text");
        b.Property(e => e.PendingSettingsJson).HasColumnType("jsonb");
        b.Property(e => e.PendingEncryptedSecrets).HasColumnType("text");
        b.Property(e => e.ReviewStatus).HasConversion<string>().HasMaxLength(30);
        b.Property(e => e.SubmittedBy).HasMaxLength(100).IsRequired();
        b.Property(e => e.ReviewedBy).HasMaxLength(100);
        b.Property(e => e.ReviewNotes).HasMaxLength(1000);
        b.HasIndex(e => new { e.OrganizationId, e.Code })
            .IsUnique()
            .HasFilter("NOT is_deleted");
        b.HasIndex(e => new { e.OrganizationId, e.ServiceCategory })
            .IsUnique()
            .HasFilter("is_enabled AND NOT is_deleted");
    }
}
