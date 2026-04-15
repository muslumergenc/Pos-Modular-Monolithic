using Pos.Shared.Common;

namespace Pos.Modules.Payments.Application.Interfaces;

/// <summary>
/// 3D Secure ödeme gateway'i ile iletişim için servis arayüzü.
/// Uygulama katmanı, altyapı detaylarından bağımsız kalır.
/// </summary>
public interface IGatewayPaymentService
{
    Task<Result<GatewayPaymentInitiateResult>> InitiatePaymentAsync(
        GatewayPaymentInitiateRequest request,
        CancellationToken ct = default);

    Task<Result<GatewayCallbackResult>> ProcessCallbackAsync(
        Dictionary<string, string> parameters,
        CancellationToken ct = default);
}

public class GatewayPaymentInitiateRequest
{
    public Guid TransactionId { get; set; }
    public Guid OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string Language { get; set; } = "tr";
    public string CardNumber { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
    public string ExpiryDateYear { get; set; } = string.Empty;
    public string ExpiryDateMonth { get; set; } = string.Empty;
    public string RequestIp { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    /// <summary>
    /// Pos.Web'in çalıştığı base URL. Örn: https://localhost:7600
    /// null ise GarantiBankOptions'taki FailUrl/SuccessUrl kullanılır.
    /// </summary>
    public string? CallbackBaseUrl { get; set; }
}

public class GatewayPaymentInitiateResult
{
    public string Markup { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}

public class GatewayCallbackResult
{
    public bool Success { get; set; }
    public string? TransactionId { get; set; }
    public string? AuthCode { get; set; }
    public string? ResponseMessage { get; set; }
    public decimal TotalAmount { get; set; }
}

