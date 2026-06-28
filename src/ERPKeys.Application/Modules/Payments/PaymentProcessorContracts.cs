namespace ERPKeys.Application.Modules.Payments;

public record PaymentAuthorizationRequest(
    string IdempotencyKey,
    string MethodOfPaymentCode,
    decimal Amount,
    string Currency,
    string PaymentToken,
    string? CustomerReference,
    string? OrderReference,
    IReadOnlyDictionary<string, string>? Metadata = null);

public record PaymentAuthorizationResult(
    bool Approved,
    string Status,
    string ProcessorReference,
    string? AuthorizationCode,
    string? DeclineCode,
    string? DeclineMessage,
    DateTimeOffset ProcessedAt);

public record PaymentCaptureRequest(
    string IdempotencyKey,
    string AuthorizationReference,
    decimal Amount,
    string Currency);

public record PaymentRefundRequest(
    string IdempotencyKey,
    string OriginalProcessorReference,
    decimal Amount,
    string Currency,
    string Reason);

public record PaymentOperationResult(
    bool Succeeded,
    string Status,
    string ProcessorReference,
    string? ErrorCode,
    string? ErrorMessage,
    DateTimeOffset ProcessedAt);

public record PaymentSettlementRequest(
    string IdempotencyKey,
    string MethodOfPaymentCode,
    DateOnly BusinessDate,
    string Currency,
    decimal GrossAmount,
    decimal RefundAmount,
    decimal FeeAmount,
    IReadOnlyList<string> ProcessorTransactionReferences);

public record PaymentSettlementResult(
    bool Accepted,
    string Status,
    string SettlementReference,
    decimal NetAmount,
    DateOnly? ExpectedDepositDate,
    string? ErrorCode,
    string? ErrorMessage);

public interface IPaymentProcessorGateway
{
    string ProviderCode { get; }
    Task<PaymentAuthorizationResult> AuthorizeAsync(
        PaymentAuthorizationRequest request, CancellationToken ct = default);
    Task<PaymentOperationResult> CaptureAsync(
        PaymentCaptureRequest request, CancellationToken ct = default);
    Task<PaymentOperationResult> VoidAsync(
        string idempotencyKey, string processorReference, CancellationToken ct = default);
    Task<PaymentOperationResult> RefundAsync(
        PaymentRefundRequest request, CancellationToken ct = default);
    Task<PaymentSettlementResult> SubmitSettlementAsync(
        PaymentSettlementRequest request, CancellationToken ct = default);
}

public interface IPaymentProcessorGatewayResolver
{
    IPaymentProcessorGateway Resolve(string providerCode);
}

public class PaymentProcessorGatewayResolver : IPaymentProcessorGatewayResolver
{
    private readonly IReadOnlyDictionary<string, IPaymentProcessorGateway> _gateways;

    public PaymentProcessorGatewayResolver(IEnumerable<IPaymentProcessorGateway> gateways)
    {
        _gateways = gateways.ToDictionary(
            gateway => gateway.ProviderCode,
            StringComparer.OrdinalIgnoreCase);
    }

    public IPaymentProcessorGateway Resolve(string providerCode)
    {
        if (_gateways.TryGetValue(providerCode, out var gateway)) return gateway;
        throw new InvalidOperationException(
            $"No payment processor gateway is registered for provider '{providerCode}'.");
    }
}
