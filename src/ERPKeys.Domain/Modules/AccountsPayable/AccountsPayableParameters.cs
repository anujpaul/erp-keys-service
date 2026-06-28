using ERPKeys.Domain.Common;

namespace ERPKeys.Domain.Modules.AccountsPayable;

public class AccountsPayableParameters : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public bool AllowPurchaseOrderOverReceipt { get; private set; }
    public decimal MaximumOverReceiptPercent { get; private set; }

    private AccountsPayableParameters() { }

    public AccountsPayableParameters(Guid organizationId)
    {
        OrganizationId = organizationId;
    }

    public void UpdateOverReceiptPolicy(bool allowOverReceipt, decimal maximumPercent)
    {
        if (maximumPercent < 0 || maximumPercent > 100)
            throw new InvalidOperationException(
                "Maximum over-receipt percentage must be between 0 and 100.");

        AllowPurchaseOrderOverReceipt = allowOverReceipt;
        MaximumOverReceiptPercent = allowOverReceipt ? maximumPercent : 0;
        SetUpdated();
    }
}
