using MediatR;
using Pos.Modules.Payments.Application.DTOs;
using Pos.Shared.Common;

namespace Pos.Modules.Payments.Application.Commands;

/// <summary>
/// 3D Secure ödeme başlatma komutu.
/// Handler, ödeme kaydı oluşturur ve banka yönlendirme HTML'ini döner.
/// </summary>
public record InitiateGatewayPaymentCommand(
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
    string? CallbackBaseUrl) : IRequest<Result<GatewayPaymentMarkupDto>>;

/// <summary>
/// Bankadan gelen callback'i işleme komutu.
/// Handler, ödeme kaydını Completed/Failed olarak günceller.
/// </summary>
public record ProcessGatewayCallbackCommand(
    Guid PaymentId,
    Dictionary<string, string> Parameters) : IRequest<Result<GatewayCallbackResultDto>>;

