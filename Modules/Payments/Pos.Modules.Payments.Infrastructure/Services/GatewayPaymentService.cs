using Pos.Modules.Payments.Application.Interfaces;
using Pos.Providers.GarantiBank;
using Pos.Shared.Common;

namespace Pos.Modules.Payments.Infrastructure.Services;

public class GatewayPaymentService : IGatewayPaymentService
{
    private readonly IGarantiBankPaymentProvider _provider;

    public GatewayPaymentService(IGarantiBankPaymentProvider provider)
        => _provider = provider;

    public async Task<Result<GatewayPaymentInitiateResult>> InitiatePaymentAsync(
        GatewayPaymentInitiateRequest request, CancellationToken ct = default)
    {
        var providerRequest = new PreparePaymentRequest
        {
            TransactionId   = request.TransactionId,
            OrderId         = request.OrderId,
            TotalAmount     = request.TotalAmount,
            Currency        = request.Currency,
            Language        = request.Language,
            CardNumber      = request.CardNumber,
            Cvv             = request.Cvv,
            ExpiryDateYear  = request.ExpiryDateYear,
            ExpiryDateMonth = request.ExpiryDateMonth,
            RequestIp       = request.RequestIp,
            Email           = request.Email,
            CardHolderName  = request.CardHolderName,
            // CallbackBaseUrl sağlanmışsa provider'daki appsettings değerlerini ezer
            CallbackBaseUrl = request.CallbackBaseUrl
        };

        var result = await _provider.PreparePayment(providerRequest);
        if (result.Status != OperationResultStatus.Success)
            return Result<GatewayPaymentInitiateResult>.Failure(
                result.Message ?? "Ödeme başlatılamadı.");

        return Result<GatewayPaymentInitiateResult>.Success(new GatewayPaymentInitiateResult
        {
            Markup   = result.Data!.Markup,
            Provider = result.Data.Provider.ToString()
        });
    }

    public async Task<Result<GatewayCallbackResult>> ProcessCallbackAsync(
        Dictionary<string, string> parameters, CancellationToken ct = default)
    {
        var result = await _provider.BankCallback(parameters);
        if (result.Status != OperationResultStatus.Success)
            return Result<GatewayCallbackResult>.Failure(
                result.Message ?? "Geri arama işlenemedi.");

        return Result<GatewayCallbackResult>.Success(new GatewayCallbackResult
        {
            Success         = result.Data!.Success,
            TransactionId   = result.Data.Parameters.TransactionId,
            AuthCode        = result.Data.Parameters.AuthCode,
            ResponseMessage = result.Data.Parameters.ResponseMessage,
            TotalAmount     = result.Data.Parameters.TotalAmount
        });
    }
}

