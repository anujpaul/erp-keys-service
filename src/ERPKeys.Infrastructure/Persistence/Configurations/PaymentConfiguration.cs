using ERPKeys.Domain.Modules.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPKeys.Infrastructure.Persistence.Configurations;

public class PaymentProcessorConfigurationConfiguration
    : IEntityTypeConfiguration<PaymentProcessorConfiguration>
{
    public void Configure(EntityTypeBuilder<PaymentProcessorConfiguration> b)
    {
        b.ToTable("payment_processor_configurations");
        b.HasKey(e => e.Id);
        b.Property(e => e.OrganizationId).IsRequired();
        b.Property(e => e.Code).HasMaxLength(30).IsRequired();
        b.Property(e => e.Name).HasMaxLength(150).IsRequired();
        b.Property(e => e.ProviderCode).HasMaxLength(50).IsRequired();
        b.Property(e => e.Environment).HasConversion<string>().HasMaxLength(20);
        b.Property(e => e.EndpointBaseUrl).HasMaxLength(500).IsRequired();
        b.Property(e => e.MerchantAccountReference).HasMaxLength(200).IsRequired();
        b.Property(e => e.CredentialSecretReference).HasMaxLength(500).IsRequired();
        b.HasIndex(e => new { e.OrganizationId, e.Code }).IsUnique();
    }
}

public class MethodOfPaymentConfiguration : IEntityTypeConfiguration<MethodOfPayment>
{
    public void Configure(EntityTypeBuilder<MethodOfPayment> b)
    {
        b.ToTable("methods_of_payment");
        b.HasKey(e => e.Id);
        b.Property(e => e.OrganizationId).IsRequired();
        b.Property(e => e.Code).HasMaxLength(30).IsRequired();
        b.Property(e => e.Name).HasMaxLength(150).IsRequired();
        b.Property(e => e.Description).HasMaxLength(500);
        b.Property(e => e.Usage).HasConversion<string>().HasMaxLength(20);
        b.Property(e => e.TenderType).HasConversion<string>().HasMaxLength(30);
        b.Property(e => e.CurrencyCode).HasMaxLength(3);
        b.Property(e => e.SettlementMode).HasConversion<string>().HasMaxLength(20);
        b.Property(e => e.SettlementCutoffTime).HasColumnType("time without time zone");
        b.HasIndex(e => new { e.OrganizationId, e.Code }).IsUnique();
        b.HasOne(e => e.ProcessorConfiguration).WithMany()
            .HasForeignKey(e => e.ProcessorConfigurationId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.SettlementBankAccount).WithMany()
            .HasForeignKey(e => e.SettlementBankAccountId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.ClearingAccount).WithMany()
            .HasForeignKey(e => e.ClearingAccountId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.FeeExpenseAccount).WithMany()
            .HasForeignKey(e => e.FeeExpenseAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
