using ERPKeys.Application.Common.Interfaces;
using ERPKeys.Domain.Modules.Organization;
using Microsoft.EntityFrameworkCore;

namespace ERPKeys.Application.Common.Services;

public static class NumberSequenceAreas
{
    public const string Customer = "customer";
    public const string Vendor = "vendor";
    public const string SalesOrder = "sales-order";
    public const string SalesQuotation = "sales-quotation";
    public const string ArInvoice = "ar-invoice";
    public const string ArPayment = "ar-payment";
    public const string CustomerCreditNote = "customer-credit-note";
    public const string Dunning = "dunning";
    public const string PurchaseOrder = "purchase-order";
    public const string PurchaseRequisition = "purchase-requisition";
    public const string GoodsReceipt = "goods-receipt";
    public const string ApInvoice = "ap-invoice";
    public const string ApPayment = "ap-payment";
    public const string PaymentProposal = "payment-proposal";
    public const string VendorCreditNote = "vendor-credit-note";
    public const string JournalEntry = "journal-entry";
    public const string CashJournal = "cash-journal";
    public const string BankTransaction = "bank-transaction";
    public const string BankReconciliation = "bank-reconciliation";
    public const string ExpenseReport = "expense-report";

    public static readonly IReadOnlyList<NumberSequenceDefault> Defaults =
    [
        new(Customer, "Customer", "CUST", false, 5),
        new(Vendor, "Vendor", "VEND", false, 5),
        new(SalesOrder, "Sales order", "SO", true, 5),
        new(SalesQuotation, "Sales quotation", "QUO", true, 5),
        new(ArInvoice, "Customer invoice", "INV", false, 6),
        new(ArPayment, "Customer receipt", "RCPT", false, 6),
        new(CustomerCreditNote, "Customer credit note", "CN", false, 6),
        new(Dunning, "Dunning notice", "DUN", false, 5),
        new(PurchaseOrder, "Purchase order", "PO", true, 5),
        new(PurchaseRequisition, "Purchase requisition", "PR", true, 5),
        new(GoodsReceipt, "Goods receipt", "GRN", false, 6),
        new(ApInvoice, "Vendor invoice", "APINV", false, 6),
        new(ApPayment, "Vendor payment", "PMT", false, 6),
        new(PaymentProposal, "Payment proposal", "PAY", true, 5),
        new(VendorCreditNote, "Vendor credit note", "VCN", false, 6),
        new(JournalEntry, "Journal entry", "JE", false, 6),
        new(CashJournal, "Cash journal", "CJ", false, 6),
        new(BankTransaction, "Bank transaction", "TXN", false, 7),
        new(BankReconciliation, "Bank reconciliation", "REC", false, 5),
        new(ExpenseReport, "Expense report", "EXP", false, 4)
    ];
}

public sealed record NumberSequenceDefault(
    string Area,
    string DisplayName,
    string Prefix,
    bool IncludeYear,
    int Padding);

public sealed record NumberSequenceDto(
    Guid Id,
    string Area,
    string DisplayName,
    string Prefix,
    bool IncludeYear,
    string Separator,
    int Padding,
    long NextNumber,
    bool AllowManualOverride,
    bool IsActive,
    string Preview);

public sealed record UpdateNumberSequenceRequest(
    string DisplayName,
    string Prefix,
    bool IncludeYear,
    string Separator,
    int Padding,
    long NextNumber,
    bool AllowManualOverride,
    bool IsActive);

public interface INumberSequenceService
{
    Task<IReadOnlyList<NumberSequenceDto>> GetSequencesAsync(CancellationToken ct = default);
    Task<NumberSequenceDto> UpdateSequenceAsync(string area, UpdateNumberSequenceRequest req, CancellationToken ct = default);
    Task<string> ReserveNextAsync(string area, DateTime documentDate, CancellationToken ct = default);
    Task EnsureDefaultSequencesAsync(CancellationToken ct = default);
}

public class NumberSequenceService : INumberSequenceService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentOrganizationService _org;

    public NumberSequenceService(IAppDbContext db, ICurrentOrganizationService org)
    {
        _db = db;
        _org = org;
    }

    public async Task<IReadOnlyList<NumberSequenceDto>> GetSequencesAsync(CancellationToken ct = default)
    {
        await EnsureDefaultSequencesAsync(ct);
        var sequences = await _db.NumberSequences
            .Where(sequence => sequence.OrganizationId == _org.OrganizationId)
            .OrderBy(sequence => sequence.DisplayName)
            .ToListAsync(ct);
        return sequences.Select(ToDto).ToList();
    }

    public async Task<NumberSequenceDto> UpdateSequenceAsync(
        string area,
        UpdateNumberSequenceRequest req,
        CancellationToken ct = default)
    {
        await EnsureDefaultSequencesAsync(ct);
        var normalizedArea = NormalizeArea(area);
        var sequence = await _db.NumberSequences
            .FirstOrDefaultAsync(s => s.OrganizationId == _org.OrganizationId && s.Area == normalizedArea, ct)
            ?? throw new InvalidOperationException("Number sequence not found.");

        sequence.Update(
            req.DisplayName,
            req.Prefix,
            req.IncludeYear,
            req.Separator,
            req.Padding,
            req.NextNumber,
            req.AllowManualOverride,
            req.IsActive);

        await _db.SaveChangesAsync(ct);
        return ToDto(sequence);
    }

    public async Task<string> ReserveNextAsync(string area, DateTime documentDate, CancellationToken ct = default)
    {
        await EnsureDefaultSequencesAsync(ct);
        var normalizedArea = NormalizeArea(area);
        await using var tx = await _db.BeginTransactionAsync(ct);
        var sequence = await _db.NumberSequences
            .FirstOrDefaultAsync(s => s.OrganizationId == _org.OrganizationId && s.Area == normalizedArea, ct)
            ?? throw new InvalidOperationException($"Number sequence '{area}' is not configured.");
        var next = sequence.ReserveNext(documentDate);
        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return next;
    }

    public async Task EnsureDefaultSequencesAsync(CancellationToken ct = default)
    {
        if (_org.OrganizationId == Guid.Empty) return;

        var existingAreas = await _db.NumberSequences
            .Where(sequence => sequence.OrganizationId == _org.OrganizationId)
            .Select(sequence => sequence.Area)
            .ToListAsync(ct);
        var existing = existingAreas.ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var def in NumberSequenceAreas.Defaults)
        {
            if (existing.Contains(def.Area)) continue;
            _db.NumberSequences.Add(new NumberSequence(
                _org.OrganizationId,
                def.Area,
                def.DisplayName,
                def.Prefix,
                def.Padding,
                await EstimateNextNumberAsync(def.Area, ct),
                includeYear: def.IncludeYear));
        }

        await _db.SaveChangesAsync(ct);
    }

    private static NumberSequenceDto ToDto(NumberSequence sequence) =>
        new(
            sequence.Id,
            sequence.Area,
            sequence.DisplayName,
            sequence.Prefix,
            sequence.IncludeYear,
            sequence.Separator,
            sequence.Padding,
            sequence.NextNumber,
            sequence.AllowManualOverride,
            sequence.IsActive,
            sequence.Preview(DateTime.UtcNow));

    private static string NormalizeArea(string area) => area.Trim().ToLowerInvariant();

    private Task<long> EstimateNextNumberAsync(string area, CancellationToken ct) =>
        area switch
        {
            NumberSequenceAreas.Customer => CountNextAsync(_db.Customers, ct),
            NumberSequenceAreas.Vendor => CountNextAsync(_db.Vendors, ct),
            NumberSequenceAreas.SalesOrder => CountNextAsync(_db.SalesOrders, ct),
            NumberSequenceAreas.SalesQuotation => CountNextAsync(_db.SalesQuotations, ct),
            NumberSequenceAreas.ArInvoice => CountNextAsync(_db.ARInvoices, ct),
            NumberSequenceAreas.ArPayment => CountNextAsync(_db.ARPayments, ct),
            NumberSequenceAreas.CustomerCreditNote => CountNextAsync(_db.CustomerCreditNotes, ct),
            NumberSequenceAreas.Dunning => CountNextAsync(_db.DunningRecords, ct),
            NumberSequenceAreas.PurchaseOrder => CountNextAsync(_db.PurchaseOrders, ct),
            NumberSequenceAreas.PurchaseRequisition => CountNextAsync(_db.PurchaseRequisitions, ct),
            NumberSequenceAreas.GoodsReceipt => CountNextAsync(_db.PurchaseOrderReceipts, ct),
            NumberSequenceAreas.ApInvoice => CountNextAsync(_db.APInvoices, ct),
            NumberSequenceAreas.ApPayment => CountNextAsync(_db.APPayments, ct),
            NumberSequenceAreas.PaymentProposal => CountNextAsync(_db.PaymentProposals, ct),
            NumberSequenceAreas.VendorCreditNote => CountNextAsync(_db.VendorCreditNotes, ct),
            NumberSequenceAreas.JournalEntry => CountNextAsync(_db.JournalEntries, ct),
            NumberSequenceAreas.CashJournal => CountNextAsync(_db.CashJournals, ct),
            NumberSequenceAreas.BankTransaction => CountNextAsync(_db.BankTransactions, ct),
            NumberSequenceAreas.BankReconciliation => CountNextAsync(_db.BankReconciliations, ct),
            NumberSequenceAreas.ExpenseReport => CountNextAsync(_db.ExpenseReports, ct),
            _ => Task.FromResult(1L)
        };

    private async Task<long> CountNextAsync<T>(IQueryable<T> query, CancellationToken ct)
        where T : class
        => await query.CountAsync(ct) + 1L;
}
