namespace Pos.Providers.GarantiBank;

public class PreparePaymentResponse
{
    public string Markup { get; set; } = string.Empty;
    public PaymentGatewayMode Mode { get; set; }
    public ThirdPartyProvider Provider { get; set; }
}

