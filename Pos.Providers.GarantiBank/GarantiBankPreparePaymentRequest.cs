namespace Pos.Providers.GarantiBank;
public class GarantiBankPreparePaymentRequest
{
    public string? Lang { get; set; }
    public string? MerchantId { get; set; }
    public string? Mode { get; set; }
    public string? ApiVersion { get; set; }
    public string? TerminalUserId { get; set; }
    public string? TerminalProvUserId { get; set; }
    public string? SecurityLevel { get; set; }
    public string? Amount { get; set; }
    public string? OkUrl { get; set; }
    public string? FailUrl { get; set; }
    public string? TxnType { get; set; }
    public string? Installment { get; set; }
    public string? CallbackUrl { get; set; }
    public string? Currency { get; set; }
    public string? Storekey { get; set; }
    public string? Pan { get; set; }
    public string? PanExpireMonth { get; set; }
    public string? PanExpireYear { get; set; }
    public string? CV2 { get; set; }
    public string? Hash { get; set; }
    public string? CustomerIp { get; set; }
    public string? CustomerEmail { get; set; }
}