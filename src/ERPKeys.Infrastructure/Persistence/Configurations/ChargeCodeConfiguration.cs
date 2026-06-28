using ERPKeys.Domain.Modules.GeneralLedger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPKeys.Infrastructure.Persistence.Configurations;

public class ChargeCodeConfiguration : IEntityTypeConfiguration<ChargeCode>
{
    public void Configure(EntityTypeBuilder<ChargeCode> builder)
    {
        builder.ToTable("charge_codes");
        builder.HasKey(charge => charge.Id);
        builder.Property(charge => charge.OrganizationId).IsRequired();
        builder.Property(charge => charge.Module)
            .HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(charge => charge.Code).HasMaxLength(30).IsRequired();
        builder.Property(charge => charge.Name).HasMaxLength(150).IsRequired();
        builder.Property(charge => charge.Description).HasMaxLength(500);
        builder.Property(charge => charge.CalculationMethod)
            .HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(charge => charge.DefaultValue).HasColumnType("numeric(18,4)");
        builder.Property(charge => charge.CurrencyCode).HasMaxLength(3);
        builder.HasOne(charge => charge.PostingAccount)
            .WithMany()
            .HasForeignKey(charge => charge.PostingAccountId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(charge => new
        {
            charge.OrganizationId,
            charge.Module,
            charge.Code
        }).IsUnique();
    }
}
