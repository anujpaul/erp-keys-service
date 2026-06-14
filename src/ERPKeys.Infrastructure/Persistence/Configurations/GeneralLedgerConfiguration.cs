using ERPKeys.Domain.Modules.GeneralLedger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPKeys.Infrastructure.Persistence.Configurations;

public class FiscalCalendarConfiguration : IEntityTypeConfiguration<FiscalCalendar>
{
    public void Configure(EntityTypeBuilder<FiscalCalendar> b)
    {
        b.ToTable("fiscal_calendars");
        b.HasKey(e => e.Id);
        b.Property(e => e.OrganizationId).IsRequired();
        b.Property(e => e.Name).HasMaxLength(100).IsRequired();
        b.Property(e => e.Description).HasMaxLength(500);
        b.Property(e => e.CalendarType).HasMaxLength(20).IsRequired();
        b.HasIndex(e => new { e.OrganizationId, e.Name }).IsUnique();
        b.HasIndex(e => e.OrganizationId)
            .IsUnique()
            .HasFilter("is_default = TRUE AND is_deleted = FALSE");
        b.HasMany(e => e.FiscalYears)
            .WithOne(e => e.FiscalCalendar)
            .HasForeignKey(e => e.FiscalCalendarId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class FiscalYearConfiguration : IEntityTypeConfiguration<FiscalYear>
{
    public void Configure(EntityTypeBuilder<FiscalYear> b)
    {
        b.ToTable("fiscal_years");
        b.HasKey(e => e.Id);
        b.Property(e => e.OrganizationId).IsRequired();
        b.Property(e => e.FiscalCalendarId).IsRequired();
        b.Property(e => e.Name).HasMaxLength(100).IsRequired();
        b.Property(e => e.Description).HasMaxLength(500);
        b.Property(e => e.CalendarType).HasMaxLength(50);
        b.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        // Query filter is applied in AppDbContext.OnModelCreating
        b.HasMany(e => e.Periods).WithOne(p => p.FiscalYear)
            .HasForeignKey(p => p.FiscalYearId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(e => new { e.FiscalCalendarId, e.Name }).IsUnique();
    }
}

public class FiscalPeriodConfiguration : IEntityTypeConfiguration<FiscalPeriod>
{
    public void Configure(EntityTypeBuilder<FiscalPeriod> b)
    {
        b.ToTable("fiscal_periods");
        b.HasKey(e => e.Id);
        b.Property(e => e.Name).HasMaxLength(100).IsRequired();
        b.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        b.HasQueryFilter(e => !e.IsDeleted);
    }
}

public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
{
    public void Configure(EntityTypeBuilder<AccountType> b)
    {
        b.ToTable("account_types");
        b.HasKey(e => e.Id);
        b.Property(e => e.Code).HasMaxLength(20).IsRequired();
        b.Property(e => e.Name).HasMaxLength(100).IsRequired();
        b.Property(e => e.Nature).HasConversion<string>().HasMaxLength(10);
        // AccountType is shared across orgs (no org filter)
        b.HasQueryFilter(e => !e.IsDeleted);
    }
}

public class ChartOfAccountsConfiguration : IEntityTypeConfiguration<ChartOfAccounts>
{
    public void Configure(EntityTypeBuilder<ChartOfAccounts> b)
    {
        b.ToTable("charts_of_accounts");
        b.HasKey(e => e.Id);
        b.Property(e => e.OrganizationId).IsRequired();
        b.Property(e => e.Code).HasMaxLength(30).IsRequired();
        b.Property(e => e.Name).HasMaxLength(150).IsRequired();
        b.Property(e => e.Description).HasMaxLength(500);
        b.HasIndex(e => new { e.OrganizationId, e.Code }).IsUnique();
        b.HasIndex(e => e.OrganizationId).IsUnique()
            .HasFilter("is_default = TRUE AND is_deleted = FALSE");
        b.HasMany(e => e.Accounts).WithOne(e => e.ChartOfAccounts)
            .HasForeignKey(e => e.ChartOfAccountsId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class LedgerConfiguration : IEntityTypeConfiguration<Ledger>
{
    public void Configure(EntityTypeBuilder<Ledger> b)
    {
        b.ToTable("ledgers");
        b.HasKey(e => e.Id);
        b.Property(e => e.OrganizationId).IsRequired();
        b.Property(e => e.Code).HasMaxLength(30).IsRequired();
        b.Property(e => e.Name).HasMaxLength(150).IsRequired();
        b.Property(e => e.Description).HasMaxLength(500);
        b.HasIndex(e => new { e.OrganizationId, e.Code }).IsUnique();
        b.HasIndex(e => e.OrganizationId).IsUnique()
            .HasFilter("is_default = TRUE AND is_deleted = FALSE");
        b.HasOne(e => e.FunctionalCurrency).WithMany()
            .HasForeignKey(e => e.FunctionalCurrencyId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.ReportingCurrency).WithMany()
            .HasForeignKey(e => e.ReportingCurrencyId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.FiscalCalendar).WithMany()
            .HasForeignKey(e => e.FiscalCalendarId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.ChartOfAccounts).WithMany()
            .HasForeignKey(e => e.ChartOfAccountsId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class GeneralLedgerParametersConfiguration : IEntityTypeConfiguration<GeneralLedgerParameters>
{
    public void Configure(EntityTypeBuilder<GeneralLedgerParameters> b)
    {
        b.ToTable("general_ledger_parameters");
        b.HasKey(e => e.Id);
        b.Property(e => e.OrganizationId).IsRequired();
        b.Property(e => e.MaximumPennyDifference).HasColumnType("numeric(18,4)");
        b.Property(e => e.DefaultJournalType).HasMaxLength(50).IsRequired();
        b.HasIndex(e => e.OrganizationId).IsUnique();
        b.HasOne(e => e.DefaultLedger).WithMany()
            .HasForeignKey(e => e.DefaultLedgerId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.DefaultFinancialDimensionSet).WithMany()
            .HasForeignKey(e => e.DefaultFinancialDimensionSetId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.RetainedEarningsAccount).WithMany()
            .HasForeignKey(e => e.RetainedEarningsAccountId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.RoundingDifferenceAccount).WithMany()
            .HasForeignKey(e => e.RoundingDifferenceAccountId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.RealizedGainAccount).WithMany()
            .HasForeignKey(e => e.RealizedGainAccountId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.RealizedLossAccount).WithMany()
            .HasForeignKey(e => e.RealizedLossAccountId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.UnrealizedGainAccount).WithMany()
            .HasForeignKey(e => e.UnrealizedGainAccountId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.UnrealizedLossAccount).WithMany()
            .HasForeignKey(e => e.UnrealizedLossAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> b)
    {
        b.ToTable("accounts");
        b.HasKey(e => e.Id);
        b.Property(e => e.OrganizationId).IsRequired();
        b.Property(e => e.ChartOfAccountsId).IsRequired();
        b.Property(e => e.AccountNumber).HasMaxLength(20).IsRequired();
        b.Property(e => e.Name).HasMaxLength(200).IsRequired();
        b.Property(e => e.Description).HasMaxLength(500);
        b.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        b.Property(e => e.Currency).HasMaxLength(3);
        // Unique per org (same number can exist in different orgs)
        b.HasIndex(e => new { e.ChartOfAccountsId, e.AccountNumber }).IsUnique();
        b.HasOne(e => e.AccountType).WithMany().HasForeignKey(e => e.AccountTypeId);
        b.HasOne(e => e.ParentAccount).WithMany().HasForeignKey(e => e.ParentAccountId);
        // Query filter applied in AppDbContext.OnModelCreating
    }
}

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> b)
    {
        b.ToTable("journal_entries");
        b.HasKey(e => e.Id);
        b.Property(e => e.OrganizationId).IsRequired();
        b.Property(e => e.LedgerId).IsRequired();
        b.Property(e => e.EntryNumber).HasMaxLength(20).IsRequired();
        b.Property(e => e.Description).HasMaxLength(500);
        b.Property(e => e.Reference).HasMaxLength(100);
        b.Property(e => e.JournalType).HasMaxLength(50);
        b.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        b.Property(e => e.Currency).HasMaxLength(3);
        b.Property(e => e.TotalDebit).HasColumnType("numeric(18,4)");
        b.Property(e => e.TotalCredit).HasColumnType("numeric(18,4)");
        b.HasOne(e => e.FiscalPeriod).WithMany().HasForeignKey(e => e.FiscalPeriodId);
        b.HasOne(e => e.Ledger).WithMany().HasForeignKey(e => e.LedgerId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasMany(e => e.Lines).WithOne(l => l.JournalEntry)
            .HasForeignKey(l => l.JournalEntryId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(e => new { e.OrganizationId, e.EntryNumber }).IsUnique();
        // Query filter applied in AppDbContext.OnModelCreating
    }
}

public class JournalLineConfiguration : IEntityTypeConfiguration<JournalLine>
{
    public void Configure(EntityTypeBuilder<JournalLine> b)
    {
        b.ToTable("journal_lines");
        b.HasKey(e => e.Id);
        b.Property(e => e.Description).HasMaxLength(500);
        b.Property(e => e.Debit).HasColumnType("numeric(18,4)");
        b.Property(e => e.Credit).HasColumnType("numeric(18,4)");
        b.HasOne(e => e.Account).WithMany().HasForeignKey(e => e.AccountId);
        b.HasOne(e => e.FinancialDimensionSet).WithMany()
            .HasForeignKey(e => e.FinancialDimensionSetId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasMany(e => e.DimensionValues).WithOne(e => e.JournalLine)
            .HasForeignKey(e => e.JournalLineId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasQueryFilter(e => !e.IsDeleted);
    }
}

public class FinancialDimensionConfiguration : IEntityTypeConfiguration<FinancialDimension>
{
    public void Configure(EntityTypeBuilder<FinancialDimension> b)
    {
        b.ToTable("financial_dimensions");
        b.HasKey(e => e.Id);
        b.Property(e => e.Code).HasMaxLength(30).IsRequired();
        b.Property(e => e.Name).HasMaxLength(100).IsRequired();
        b.Property(e => e.Description).HasMaxLength(500);
        b.HasIndex(e => new { e.OrganizationId, e.Code }).IsUnique();
        b.HasMany(e => e.Values).WithOne(e => e.FinancialDimension)
            .HasForeignKey(e => e.FinancialDimensionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class FinancialDimensionValueConfiguration : IEntityTypeConfiguration<FinancialDimensionValue>
{
    public void Configure(EntityTypeBuilder<FinancialDimensionValue> b)
    {
        b.ToTable("financial_dimension_values");
        b.HasKey(e => e.Id);
        b.Property(e => e.Code).HasMaxLength(50).IsRequired();
        b.Property(e => e.Name).HasMaxLength(150).IsRequired();
        b.Property(e => e.Description).HasMaxLength(500);
        b.HasIndex(e => new { e.FinancialDimensionId, e.Code }).IsUnique();
        b.HasQueryFilter(e => !e.IsDeleted);
    }
}

public class FinancialDimensionSetConfiguration : IEntityTypeConfiguration<FinancialDimensionSet>
{
    public void Configure(EntityTypeBuilder<FinancialDimensionSet> b)
    {
        b.ToTable("financial_dimension_sets");
        b.HasKey(e => e.Id);
        b.Property(e => e.Name).HasMaxLength(100).IsRequired();
        b.Property(e => e.Description).HasMaxLength(500);
        b.HasIndex(e => new { e.OrganizationId, e.Name }).IsUnique();
        b.HasIndex(e => e.OrganizationId)
            .IsUnique()
            .HasFilter("is_default = TRUE AND is_deleted = FALSE");
        b.HasMany(e => e.Members).WithOne(e => e.FinancialDimensionSet)
            .HasForeignKey(e => e.FinancialDimensionSetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class FinancialDimensionSetMemberConfiguration : IEntityTypeConfiguration<FinancialDimensionSetMember>
{
    public void Configure(EntityTypeBuilder<FinancialDimensionSetMember> b)
    {
        b.ToTable("financial_dimension_set_members");
        b.HasKey(e => e.Id);
        b.HasIndex(e => new { e.FinancialDimensionSetId, e.FinancialDimensionId }).IsUnique();
        b.HasOne(e => e.FinancialDimension).WithMany()
            .HasForeignKey(e => e.FinancialDimensionId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasQueryFilter(e => !e.IsDeleted);
    }
}

public class JournalLineDimensionValueConfiguration : IEntityTypeConfiguration<JournalLineDimensionValue>
{
    public void Configure(EntityTypeBuilder<JournalLineDimensionValue> b)
    {
        b.ToTable("journal_line_dimension_values");
        b.HasKey(e => e.Id);
        b.HasIndex(e => new { e.JournalLineId, e.FinancialDimensionValueId }).IsUnique();
        b.HasOne(e => e.FinancialDimensionValue).WithMany()
            .HasForeignKey(e => e.FinancialDimensionValueId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasQueryFilter(e => !e.IsDeleted);
    }
}

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> b)
    {
        b.ToTable("currencies");
        b.HasKey(e => e.Id);
        b.Property(e => e.OrganizationId).IsRequired();
        b.Property(e => e.Code).HasMaxLength(3).IsRequired();
        b.Property(e => e.Name).HasMaxLength(100).IsRequired();
        b.Property(e => e.Symbol).HasMaxLength(10).IsRequired();
        b.Property(e => e.DecimalPlaces).IsRequired();
        b.Property(e => e.ExchangeRate).HasColumnType("numeric(18,6)").IsRequired();
        b.Property(e => e.IsBase).IsRequired();
        b.Property(e => e.IsActive).IsRequired();
        b.Property(e => e.NumericCode);
        b.Property(e => e.Country).HasMaxLength(100);
        b.Property(e => e.RateUpdatedAt);
        // Unique per org: only one row per ISO code per organisation
        b.HasIndex(e => new { e.OrganizationId, e.Code }).IsUnique();
        // Query filter applied in AppDbContext.OnModelCreating
    }
}
