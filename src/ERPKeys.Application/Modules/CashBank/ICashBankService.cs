using ERPKeys.Application.Common.Models;
using ERPKeys.Application.Modules.CashBank.DTOs;

namespace ERPKeys.Application.Modules.CashBank;

public interface ICashBankService
{
    // ── Bank Accounts ─────────────────────────────────────────────────────────
    Task<IEnumerable<BankAccountSummaryDto>> GetBankAccountsAsync(bool activeOnly = false, CancellationToken ct = default);
    Task<BankAccountDto?> GetBankAccountAsync(Guid id, CancellationToken ct = default);
    Task<BankAccountDto> CreateBankAccountAsync(CreateBankAccountRequest req, CancellationToken ct = default);
    Task<BankAccountDto> UpdateBankAccountAsync(Guid id, UpdateBankAccountRequest req, CancellationToken ct = default);
    Task ActivateBankAccountAsync(Guid id, CancellationToken ct = default);
    Task DeactivateBankAccountAsync(Guid id, CancellationToken ct = default);

    // ── Bank Transactions ─────────────────────────────────────────────────────
    Task<PagedResult<BankTransactionSummaryDto>> GetTransactionsAsync(
        Guid? bankAccountId = null, string? status = null, string? search = null,
        int page = 1, int pageSize = 25, CancellationToken ct = default);
    Task<BankTransactionDto?> GetTransactionAsync(Guid id, CancellationToken ct = default);
    Task<BankTransactionDto> CreateTransactionAsync(CreateBankTransactionRequest req, CancellationToken ct = default);
    Task<BankTransactionDto> PostTransactionAsync(Guid id, PostTransactionRequest req, CancellationToken ct = default);
    Task VoidTransactionAsync(Guid id, CancellationToken ct = default);

    // ── Bank Reconciliation ───────────────────────────────────────────────────
    Task<PagedResult<BankReconciliationSummaryDto>> GetReconciliationsAsync(
        Guid? bankAccountId = null, string? status = null, string? search = null,
        int page = 1, int pageSize = 25, CancellationToken ct = default);
    Task<BankReconciliationDto?> GetReconciliationAsync(Guid id, CancellationToken ct = default);
    Task<BankReconciliationDto> CreateReconciliationAsync(CreateReconciliationRequest req, CancellationToken ct = default);
    Task<BankReconciliationDto> ReconcileTransactionAsync(Guid reconciliationId, ReconcileTransactionRequest req, CancellationToken ct = default);
    Task<BankReconciliationDto> CompleteReconciliationAsync(Guid id, CompleteReconciliationRequest req, CancellationToken ct = default);
    Task CancelReconciliationAsync(Guid id, CancellationToken ct = default);

    // ── Cash Journals ─────────────────────────────────────────────────────────
    Task<PagedResult<CashJournalSummaryDto>> GetCashJournalsAsync(
        Guid? bankAccountId = null, string? status = null, string? search = null,
        int page = 1, int pageSize = 25, CancellationToken ct = default);
    Task<CashJournalDto?> GetCashJournalAsync(Guid id, CancellationToken ct = default);
    Task<CashJournalDto> CreateCashJournalAsync(CreateCashJournalRequest req, CancellationToken ct = default);
    Task<CashJournalDto> AddJournalLineAsync(Guid journalId, CreateCashJournalLineRequest req, CancellationToken ct = default);
    Task RemoveJournalLineAsync(Guid journalId, Guid lineId, CancellationToken ct = default);
    Task<CashJournalDto> PostCashJournalAsync(Guid id, PostCashJournalRequest req, CancellationToken ct = default);
    Task VoidCashJournalAsync(Guid id, CancellationToken ct = default);
}
