using ERPKeys.Domain.Modules.AccountsPayable;
using ERPKeys.Domain.Modules.AccountsReceivable;
using ERPKeys.Domain.Modules.CashBank;
using ERPKeys.Domain.Modules.FixedAssets;
using ERPKeys.Domain.Modules.DataManagement;
using ERPKeys.Domain.Modules.GeneralLedger;
using ERPKeys.Domain.Modules.Organization;
using ERPKeys.Domain.Modules.ProductManagement;
using ERPKeys.Domain.Modules.Marketing;
using ERPKeys.Domain.Modules.Retail;
using ERPKeys.Domain.Modules.SystemAdmin;
using ERPKeys.Domain.Modules.Workflow;
using ERPKeys.Domain.Modules.Expenses;
using ERPKeys.Domain.Modules.WarehouseManagement;
using ERPKeys.Domain.Modules.Payments;
using ERPKeys.Domain.Modules.Rag;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ERPKeys.Application.Common.Interfaces;

public interface IAppDbContext
{
    // Organizations
    DbSet<Organization> Organizations { get; }

    // Workflow Engine
    DbSet<WorkflowTemplate>     WorkflowTemplates     { get; }
    DbSet<WorkflowTemplateStep> WorkflowTemplateSteps { get; }
    DbSet<WorkflowInstance>     WorkflowInstances     { get; }
    DbSet<WorkflowApprovalStep> WorkflowApprovalSteps { get; }

    // Expense Management
    DbSet<ExpenseCategory> ExpenseCategories { get; }
    DbSet<ExpenseReport>   ExpenseReports    { get; }
    DbSet<ExpenseLine>     ExpenseLines      { get; }

    // Product Management
    DbSet<Category>        Categories       { get; }
    DbSet<Brand>           Brands           { get; }
    DbSet<Domain.Modules.ProductManagement.Product> CatalogProducts { get; }
    DbSet<ProductVariant>  ProductVariants  { get; }
    DbSet<InventoryRecord>      InventoryRecords      { get; }
    DbSet<InventoryTransaction> InventoryTransactions { get; }
    DbSet<Warehouse> Warehouses { get; }
    DbSet<WarehouseType> WarehouseTypes { get; }
    DbSet<OperationalSite> OperationalSites { get; }
    DbSet<WarehouseLocation> WarehouseLocations { get; }
    DbSet<WarehouseInventoryBalance> WarehouseInventoryBalances { get; }
    DbSet<InboundOrder> InboundOrders { get; }
    DbSet<InboundOrderLine> InboundOrderLines { get; }
    DbSet<OutboundOrder> OutboundOrders { get; }
    DbSet<OutboundOrderLine> OutboundOrderLines { get; }
    DbSet<TransferOrder> TransferOrders { get; }
    DbSet<TransferOrderLine> TransferOrderLines { get; }

    // General Ledger
    DbSet<FiscalCalendar> FiscalCalendars { get; }
    DbSet<FiscalYear>    FiscalYears    { get; }
    DbSet<FiscalPeriod>  FiscalPeriods  { get; }
    DbSet<ChartOfAccounts> ChartsOfAccounts { get; }
    DbSet<Ledger> Ledgers { get; }
    DbSet<GeneralLedgerParameters> GeneralLedgerParameters { get; }
    DbSet<AccountType>   AccountTypes   { get; }
    DbSet<Account>       Accounts       { get; }
    DbSet<JournalEntry>  JournalEntries { get; }
    DbSet<JournalLine>   JournalLines   { get; }
    DbSet<ChargeCode> ChargeCodes { get; }
    DbSet<GeneralJournalVoucherTemplate> GeneralJournalVoucherTemplates { get; }
    DbSet<AccrualScheme> AccrualSchemes { get; }
    DbSet<AccrualSchemeAllocation> AccrualSchemeAllocations { get; }
    DbSet<AccrualPostingRun> AccrualPostingRuns { get; }
    DbSet<AccrualPostingLine> AccrualPostingLines { get; }
    DbSet<FinancialDimension> FinancialDimensions { get; }
    DbSet<FinancialDimensionValue> FinancialDimensionValues { get; }
    DbSet<FinancialDimensionSet> FinancialDimensionSets { get; }
    DbSet<FinancialDimensionSetMember> FinancialDimensionSetMembers { get; }
    DbSet<JournalLineDimensionValue> JournalLineDimensionValues { get; }
    DbSet<Currency>      Currencies     { get; }

    // Accounts Receivable
    DbSet<Customer>        Customers         { get; }
    DbSet<CustomerAddress> CustomerAddresses { get; }
    DbSet<CustomerContact> CustomerContacts  { get; }
    DbSet<SalesOrder>      SalesOrders       { get; }
    DbSet<SalesOrderLine>  SalesOrderLines   { get; }
    DbSet<ARInvoice>       ARInvoices        { get; }
    DbSet<ARPayment>       ARPayments        { get; }

    // S2C additions
    DbSet<SalesQuotation>     SalesQuotations     { get; }
    DbSet<SalesQuotationLine> SalesQuotationLines { get; }
    DbSet<CustomerCreditNote> CustomerCreditNotes { get; }
    DbSet<DunningRecord>      DunningRecords      { get; }

    // Accounts Payable
    DbSet<Vendor>                   Vendors                   { get; }
    DbSet<VendorAddress>            VendorAddresses           { get; }
    DbSet<VendorContact>            VendorContacts            { get; }
    DbSet<PurchaseOrder>            PurchaseOrders            { get; }
    DbSet<PurchaseOrderLine>        PurchaseOrderLines        { get; }
    DbSet<PurchaseOrderReceipt>     PurchaseOrderReceipts     { get; }
    DbSet<PurchaseOrderReceiptLine> PurchaseOrderReceiptLines { get; }
    DbSet<APInvoice>                APInvoices                { get; }
    DbSet<APInvoiceLine>            APInvoiceLines            { get; }
    DbSet<AccountsPayableParameters> AccountsPayableParameters { get; }
    DbSet<APPayment>                APPayments                { get; }

    // P2P additions
    DbSet<PurchaseRequisition>     PurchaseRequisitions     { get; }
    DbSet<PurchaseRequisitionLine> PurchaseRequisitionLines { get; }
    DbSet<VendorCreditNote>        VendorCreditNotes        { get; }
    DbSet<PaymentProposal>         PaymentProposals         { get; }
    DbSet<PaymentProposalLine>     PaymentProposalLines     { get; }

    // System Admin
    DbSet<AppUser>        AppUsers        { get; }
    DbSet<Role>           Roles           { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<UserRole>       UserRoles       { get; }
    DbSet<AuditLogEntry>  AuditLogs       { get; }

    // Data Management
    DbSet<ImportJob>      ImportJobs      { get; }
    DbSet<ImportJobRow>   ImportJobRows   { get; }
    DbSet<ExportJobRow>   ExportJobRows   { get; }
    DbSet<BatchJobConfig> BatchJobConfigs { get; }

    // Retail
    DbSet<RetailStore>        RetailStores        { get; }
    DbSet<POSTransaction>     POSTransactions     { get; }
    DbSet<POSTransactionLine> POSTransactionLines { get; }
    DbSet<POSPayment>         POSPayments         { get; }
    DbSet<RetailStatement>    RetailStatements    { get; }
    DbSet<RetailTenderSettlement> RetailTenderSettlements { get; }
    DbSet<RetailTransactionStaging> RetailTransactionStaging { get; }
    DbSet<RetailTransactionStagingLine> RetailTransactionStagingLines { get; }
    DbSet<RetailTransactionStagingTender> RetailTransactionStagingTenders { get; }
    DbSet<Promotion>          Promotions          { get; }
    DbSet<Coupon>             Coupons             { get; }
    DbSet<CouponRedemption>   CouponRedemptions   { get; }

    // Marketing
    DbSet<Campaign>               Campaigns               { get; }
    DbSet<LoyaltyProgram>         LoyaltyPrograms         { get; }
    DbSet<CustomerLoyaltyAccount> CustomerLoyaltyAccounts { get; }

    // Trade / Price Agreements
    DbSet<PriceAgreement> PriceAgreements { get; }

    // Cash & Bank Management
    DbSet<BankAccount>       BankAccounts       { get; }
    DbSet<BankTransaction>   BankTransactions   { get; }
    DbSet<BankReconciliation> BankReconciliations { get; }
    DbSet<CashJournal>       CashJournals       { get; }
    DbSet<CashJournalLine>   CashJournalLines   { get; }

    // Shared Payments
    DbSet<PaymentProcessorConfiguration> PaymentProcessorConfigurations { get; }
    DbSet<MethodOfPayment> MethodsOfPayment { get; }

    // Fixed Assets
    DbSet<FixedAsset>        FixedAssets        { get; }
    DbSet<AssetDepreciation> AssetDepreciations { get; }
    DbSet<AssetDisposal>     AssetDisposals     { get; }
    DbSet<AssetTransfer>     AssetTransfers     { get; }
    DbSet<AssetMaintenance>  AssetMaintenances  { get; }

    // Retrieval-augmented generation
    DbSet<DocumentChunk> DocumentChunks { get; }

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
