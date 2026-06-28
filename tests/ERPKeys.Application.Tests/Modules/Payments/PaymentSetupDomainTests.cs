using ERPKeys.Domain.Modules.Payments;
using Xunit;

namespace ERPKeys.Application.Tests.Modules.Payments;

public class PaymentSetupDomainTests
{
    [Fact]
    public void Production_processor_requires_https()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new PaymentProcessorConfiguration(
                Guid.NewGuid(),
                "PAYMENTECH",
                "Chase Paymentech",
                "CHASE_PAYMENTECH",
                PaymentProcessorEnvironment.Production,
                "http://processor.example.test",
                "merchant-reference",
                "key-vault/paymentech"));

        Assert.Contains("HTTPS", exception.Message);
    }

    [Fact]
    public void Auto_capture_requires_external_authorization()
    {
        var method = NewMethod();

        var exception = Assert.Throws<ArgumentException>(() => method.Update(
            "Credit card",
            null,
            PaymentUsage.Receivable,
            PaymentTenderType.Card,
            "USD",
            null,
            null,
            null,
            null,
            requiresExternalAuthorization: false,
            autoCapture: true,
            allowRefunds: true,
            allowManualEntry: false,
            PaymentSettlementMode.None,
            settlementDelayDays: 0,
            settlementCutoffTime: null));

        Assert.Contains("Auto-capture", exception.Message);
    }

    [Fact]
    public void Batch_settlement_requires_cutoff_time()
    {
        var method = NewMethod();

        var exception = Assert.Throws<ArgumentException>(() => method.Update(
            "Credit card",
            null,
            PaymentUsage.Receivable,
            PaymentTenderType.Card,
            "USD",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            requiresExternalAuthorization: true,
            autoCapture: true,
            allowRefunds: true,
            allowManualEntry: false,
            PaymentSettlementMode.Batch,
            settlementDelayDays: 1,
            settlementCutoffTime: null));

        Assert.Contains("cutoff time", exception.Message);
    }

    private static MethodOfPayment NewMethod() => new(
        Guid.NewGuid(),
        "CARD",
        "Credit card",
        PaymentUsage.Receivable,
        PaymentTenderType.Card);
}
