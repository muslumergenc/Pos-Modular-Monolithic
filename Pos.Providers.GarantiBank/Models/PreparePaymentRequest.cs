namespace Pos.Providers.GarantiBank;

public class PreparePaymentRequest
{
    public Guid TransactionId { get; set; }
    public Guid OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Language { get; set; } = "tr";
    public string CardNumber { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
    public string ExpiryDateYear { get; set; } = string.Empty;
    public string ExpiryDateMonth { get; set; } = string.Empty;
    public string RequestIp { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;

    /// <summary>
    /// Pos.Web'in base URL'i. Sağlanırsa FailUrl/SuccessUrl bu base'e göre kurulur.
    /// Örn: https://localhost:7601  →  https://localhost:7601/Payments/GatewayCallback/{id}
    /// null ise GarantiBankOptions değerleri kullanılır.
    /// </summary>
    public string? CallbackBaseUrl { get; set; }
}

