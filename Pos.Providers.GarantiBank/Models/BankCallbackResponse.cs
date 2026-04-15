namespace Pos.Providers.GarantiBank;

public class BankCallbackResponse
{
    public bool Success { get; set; }
    public ThirdPartyProvider Provider { get; set; }
    public BankCallbackParameters Parameters { get; set; } = new();
}

