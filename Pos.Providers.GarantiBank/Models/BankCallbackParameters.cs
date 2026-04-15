namespace Pos.Providers.GarantiBank;

public class BankCallbackParameters
{
    public string OriginalResponse { get; set; } = string.Empty;
    public string AuthCode { get; set; } = string.Empty;
    public bool Approved { get; set; }
    public string ResponseCode { get; set; } = string.Empty;
    public string ResponseMessage { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

