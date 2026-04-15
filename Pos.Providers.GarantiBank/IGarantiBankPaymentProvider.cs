namespace Pos.Providers.GarantiBank;

public interface IGarantiBankPaymentProvider
{
    ThirdPartyProvider Provider { get; }
    Task<OperationResult<PreparePaymentResponse>> PreparePayment(PreparePaymentRequest request);
    Task<OperationResult<BankCallbackResponse>> BankCallback(Dictionary<string, string> parameters);
}

