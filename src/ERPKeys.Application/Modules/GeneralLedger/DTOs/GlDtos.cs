namespace ERPKeys.Application.Modules.GeneralLedger.DTOs;

// ── Fiscal Year ───────────────────────────────────────────────────────────────
public record FiscalCalendarDto(
    Guid Id,
    string Name,
    string Description,
    string CalendarType,
    bool IsDefault,
    int FiscalYearCount,
    DateTime CreatedAt);

public record CreateFiscalCalendarRequest(
    string Name,
    string Description,
    string CalendarType,
    bool IsDefault = false);

public record ChartOfAccountsDto(
    Guid Id, string Code, string Name, string Description,
    bool IsDefault, bool IsActive, int AccountCount);

public record CreateChartOfAccountsRequest(
    string Code, string Name, string? Description = null, bool IsDefault = false);

public record LedgerDto(
    Guid Id, string Code, string Name, string Description,
    Guid FunctionalCurrencyId, string FunctionalCurrencyCode,
    Guid? ReportingCurrencyId, string? ReportingCurrencyCode,
    Guid FiscalCalendarId, string FiscalCalendarName,
    Guid ChartOfAccountsId, string ChartOfAccountsName,
    bool IsDefault, bool IsActive);

public record CreateLedgerRequest(
    string Code, string Name, Guid FunctionalCurrencyId,
    Guid FiscalCalendarId, Guid ChartOfAccountsId,
    Guid? ReportingCurrencyId = null,
    string? Description = null, bool IsDefault = false);

public record GeneralLedgerParametersDto(
    Guid Id,
    Guid DefaultLedgerId,
    string DefaultLedgerCode,
    string DefaultLedgerName,
    Guid FiscalCalendarId,
    string FiscalCalendarName,
    Guid ChartOfAccountsId,
    string ChartOfAccountsName,
    string FunctionalCurrencyCode,
    Guid? DefaultFinancialDimensionSetId,
    string? DefaultFinancialDimensionSetName,
    Guid? RetainedEarningsAccountId,
    string? RetainedEarningsAccountNumber,
    Guid? RoundingDifferenceAccountId,
    string? RoundingDifferenceAccountNumber,
    Guid? RealizedGainAccountId,
    string? RealizedGainAccountNumber,
    Guid? RealizedLossAccountId,
    string? RealizedLossAccountNumber,
    Guid? UnrealizedGainAccountId,
    string? UnrealizedGainAccountNumber,
    Guid? UnrealizedLossAccountId,
    string? UnrealizedLossAccountNumber,
    bool AllowPostingToClosedPeriods,
    bool RequireDimensionsOnJournalLines,
    decimal MaximumPennyDifference,
    string DefaultJournalType);

public record UpdateGeneralLedgerParametersRequest(
    Guid DefaultLedgerId,
    Guid? DefaultFinancialDimensionSetId,
    Guid? RetainedEarningsAccountId,
    Guid? RoundingDifferenceAccountId,
    Guid? RealizedGainAccountId,
    Guid? RealizedLossAccountId,
    Guid? UnrealizedGainAccountId,
    Guid? UnrealizedLossAccountId,
    bool AllowPostingToClosedPeriods = false,
    bool RequireDimensionsOnJournalLines = false,
    decimal MaximumPennyDifference = 0.01m,
    string DefaultJournalType = "General");

public record FiscalYearDto(Guid Id, string Name, string Description,
    Guid FiscalCalendarId, string FiscalCalendarName,
    DateTime StartDate, DateTime EndDate, string CalendarType,
    string Status, int PeriodCount, DateTime CreatedAt);

public record FiscalPeriodDto(Guid Id, Guid FiscalYearId, int PeriodNumber,
    string Name, DateTime StartDate, DateTime EndDate, string Status);

public record CreateFiscalYearRequest(
    Guid FiscalCalendarId,
    string Name, string Description,
    DateTime StartDate, DateTime EndDate,
    bool AutoGeneratePeriods = true);

/// <summary>Manually add a single custom period.</summary>
public record CreatePeriodRequest(string Name, DateTime StartDate, DateTime EndDate);

/// <summary>Bulk-generate periods using a preset calendar pattern.</summary>
/// <param name="Type">Monthly | Quarterly | 4-4-5</param>
public record GeneratePeriodsRequest(string? Type = null);

/// <summary>Rename / re-date an existing Open period.</summary>
public record UpdatePeriodRequest(string Name, DateTime StartDate, DateTime EndDate);

// ── Chart of Accounts ─────────────────────────────────────────────────────────
public record AccountTypeDto(Guid Id, string Code, string Name, string Nature, int DisplayOrder);

public record AccountDto(Guid Id, Guid ChartOfAccountsId, string AccountNumber, string Name,
    string? Description, Guid AccountTypeId, string AccountTypeName,
    Guid? ParentAccountId, string? ParentAccountName,
    bool IsHeaderAccount, bool AllowManualEntry,
    string Status, string Currency, int Level);

public record CreateAccountRequest(
    Guid? ChartOfAccountsId, string AccountNumber, string Name, Guid AccountTypeId,
    bool IsHeaderAccount, Guid? ParentAccountId = null,
    string? Description = null, string Currency = "USD");

public record FinancialDimensionValueDto(
    Guid Id, Guid FinancialDimensionId, string Code, string Name,
    string Description, bool IsActive);

public record FinancialDimensionDto(
    Guid Id, string Code, string Name, string Description, bool IsActive,
    IReadOnlyList<FinancialDimensionValueDto> Values);

public record CreateFinancialDimensionRequest(
    string Code, string Name, string? Description = null);

public record CreateFinancialDimensionValueRequest(
    string Code, string Name, string? Description = null);

public record FinancialDimensionSetMemberDto(
    Guid FinancialDimensionId, string DimensionCode, string DimensionName,
    int DisplayOrder, bool IsRequired);

public record FinancialDimensionSetDto(
    Guid Id, string Name, string Description, bool IsDefault, bool IsActive,
    IReadOnlyList<FinancialDimensionSetMemberDto> Members);

public record CreateFinancialDimensionSetMemberRequest(
    Guid FinancialDimensionId, bool IsRequired = false);

public record CreateFinancialDimensionSetRequest(
    string Name, string? Description,
    IReadOnlyList<CreateFinancialDimensionSetMemberRequest> Members,
    bool IsDefault = false);

// ── Journal Entry ─────────────────────────────────────────────────────────────
public record JournalLineDimensionDto(
    Guid FinancialDimensionId, string DimensionCode, string DimensionName,
    Guid FinancialDimensionValueId, string ValueCode, string ValueName);

public record JournalLineDto(Guid Id, Guid AccountId, string AccountNumber,
    string AccountName, string Description, decimal Debit, decimal Credit, int LineOrder,
    Guid? FinancialDimensionSetId, string? FinancialDimensionSetName,
    IReadOnlyList<JournalLineDimensionDto> Dimensions);

public record JournalEntryDto(Guid Id, Guid LedgerId, string LedgerCode,
    string EntryNumber, DateTime EntryDate,
    Guid FiscalPeriodId, string FiscalPeriodName, string Description,
    string Reference, string JournalType, string Status, string Currency,
    decimal TotalDebit, decimal TotalCredit, DateTime CreatedAt,
    IReadOnlyList<JournalLineDto> Lines);

public record CreateJournalLineRequest(Guid AccountId, string Description,
    decimal Debit, decimal Credit,
    Guid? FinancialDimensionSetId = null,
    IReadOnlyList<Guid>? FinancialDimensionValueIds = null);

public record CreateJournalEntryRequest(
    Guid LedgerId, DateTime EntryDate, Guid FiscalPeriodId,
    string Description, string Reference,
    string JournalType = "General", string Currency = "USD",
    IReadOnlyList<CreateJournalLineRequest>? Lines = null);

public record GeneralJournalVoucherTemplateDto(
    Guid Id, string Name, Guid LedgerId, string LedgerCode,
    string Description, string Reference, string JournalType,
    IReadOnlyList<CreateJournalLineRequest> Lines, DateTime CreatedAt);

public record SaveGeneralJournalVoucherTemplateRequest(
    string Name, Guid LedgerId, string Description, string Reference,
    string JournalType, IReadOnlyList<CreateJournalLineRequest> Lines);

public record AccrualSchemeAllocationDto(int PeriodOffset, decimal Percentage);

public record AccrualSchemeDto(
    Guid Id, string Code, string Name, string Description,
    Guid LedgerId, string LedgerCode,
    Guid DebitAccountId, string DebitAccountNumber, string DebitAccountName,
    Guid CreditAccountId, string CreditAccountNumber, string CreditAccountName,
    string JournalType, string AllocationMethod, int DefaultPeriodCount,
    Guid? FinancialDimensionSetId, string? FinancialDimensionSetName,
    IReadOnlyList<Guid> FinancialDimensionValueIds,
    IReadOnlyList<AccrualSchemeAllocationDto> Allocations,
    bool IsActive, DateTime CreatedAt);

public record CreateAccrualSchemeRequest(
    string Code, string Name, string? Description,
    Guid LedgerId, Guid DebitAccountId, Guid CreditAccountId,
    string JournalType, string AllocationMethod, int DefaultPeriodCount,
    Guid? FinancialDimensionSetId,
    IReadOnlyList<Guid>? FinancialDimensionValueIds,
    IReadOnlyList<decimal>? AllocationPercentages);

public record PostAccrualSchemeRequest(
    Guid StartFiscalPeriodId,
    decimal TotalAmount,
    string Reference,
    string? Description);

public record AccrualPostingLineDto(
    Guid FiscalPeriodId, string FiscalPeriodName,
    Guid JournalEntryId, string JournalEntryNumber,
    int PeriodOffset, decimal Percentage, decimal Amount);

public record AccrualPostingRunDto(
    Guid Id, Guid AccrualSchemeId, string AccrualSchemeCode,
    string Reference, string Description, decimal TotalAmount,
    DateTime PostedAt, IReadOnlyList<AccrualPostingLineDto> Lines);

// ── Currency ──────────────────────────────────────────────────────────────────
public record CurrencyDto(
    Guid     Id,
    string   Code,
    string   Name,
    string   Symbol,
    int      DecimalPlaces,
    decimal  ExchangeRate,
    bool     IsBase,
    bool     IsActive,
    int?     NumericCode,
    string?  Country,
    DateTime? RateUpdatedAt,
    DateTime CreatedAt);

public record CreateCurrencyRequest(
    string  Code,
    string  Name,
    string  Symbol,
    int     DecimalPlaces  = 2,
    decimal ExchangeRate   = 1m,
    bool    IsBase         = false,
    int?    NumericCode    = null,
    string? Country        = null);

public record UpdateCurrencyRequest(
    string  Name,
    string  Symbol,
    int     DecimalPlaces,
    string? Country,
    int?    NumericCode);

public record UpdateExchangeRateRequest(decimal ExchangeRate);

// ── Reports ───────────────────────────────────────────────────────────────────
public record TrialBalanceLineDto(string AccountNumber, string AccountName,
    string AccountType, decimal TotalDebit, decimal TotalCredit, decimal Balance);
