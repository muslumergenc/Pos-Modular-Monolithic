using Pos.Modules.Payments.Domain.Enums;

namespace Pos.Modules.Payments.Application.DTOs;

public record GatewayPaymentMarkupDto(string Markup, string Provider);

public record GatewayCallbackResultDto(
    bool Success,
    string? TransactionId,
    string? AuthCode,
    string? ResponseMessage,
    decimal TotalAmount);

public record InitiateGatewayPaymentDto(
    Guid OrderId,
    string Currency,
    string Language,
    string CardNumber,
    string Cvv,
    string ExpiryDateYear,
    string ExpiryDateMonth,
    string RequestIp,
    string Email,
    string CardHolderName,
    /// <summary>Pos.Web'in kendi host adresi. Örn: https://localhost:7600</summary>
    string? CallbackBaseUrl = null);

