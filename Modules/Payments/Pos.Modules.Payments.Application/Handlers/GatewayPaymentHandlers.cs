using MediatR;
using Pos.Modules.Payments.Application.Commands;
using Pos.Modules.Payments.Application.DTOs;
using Pos.Modules.Payments.Application.Interfaces;
using Pos.Modules.Payments.Domain.Entities;
using Pos.Modules.Payments.Domain.Enums;
using Pos.Shared.Common;

namespace Pos.Modules.Payments.Application.Handlers;

public class GatewayPaymentCommandHandler :
    IRequestHandler<InitiateGatewayPaymentCommand, Result<GatewayPaymentMarkupDto>>,
    IRequestHandler<ProcessGatewayCallbackCommand, Result<GatewayCallbackResultDto>>
{
    private readonly IPaymentRepository _repo;
    private readonly IOrderStatusService _orderStatusService;
    private readonly IGatewayPaymentService _gatewayService;

    public GatewayPaymentCommandHandler(
        IPaymentRepository repo,
        IOrderStatusService orderStatusService,
        IGatewayPaymentService gatewayService)
    {
        _repo = repo;
        _orderStatusService = orderStatusService;
        _gatewayService = gatewayService;
    }

    /// <summary>
    /// 3D Secure ödeme akışı başlatır:
    /// 1. Sipariş tutarını doğrular
    /// 2. Pending durumda Payment kaydı oluşturur
    /// 3. Garanti Bankası yönlendirme HTML'ini döner
    /// </summary>
    public async Task<Result<GatewayPaymentMarkupDto>> Handle(
        InitiateGatewayPaymentCommand req, CancellationToken ct)
    {
        var orderInfo = await _orderStatusService.GetOrderInfoAsync(req.OrderId, ct);
        if (orderInfo is null)
            return Result<GatewayPaymentMarkupDto>.Failure("Sipariş bulunamadı.");

        // Pending durumda ödeme kaydı oluştur
        var payment = Payment.Create(req.OrderId, orderInfo.Value.TotalAmount, PaymentMethod.CreditCard);
        await _repo.AddAsync(payment, ct);
        await _repo.SaveChangesAsync(ct);

        var initiateRequest = new GatewayPaymentInitiateRequest
        {
            TransactionId   = payment.Id,
            OrderId         = req.OrderId,
            TotalAmount     = orderInfo.Value.TotalAmount,
            Currency        = req.Currency,
            Language        = req.Language,
            CardNumber      = req.CardNumber,
            Cvv             = req.Cvv,
            ExpiryDateYear  = req.ExpiryDateYear,
            ExpiryDateMonth = req.ExpiryDateMonth,
            RequestIp       = req.RequestIp,
            Email           = req.Email,
            CardHolderName  = req.CardHolderName,
            CallbackBaseUrl = req.CallbackBaseUrl
        };

        var result = await _gatewayService.InitiatePaymentAsync(initiateRequest, ct);
        if (!result.IsSuccess)
        {
            payment.Fail();
            _repo.Update(payment);
            await _repo.SaveChangesAsync(ct);
            return Result<GatewayPaymentMarkupDto>.Failure(result.Error!);
        }

        return Result<GatewayPaymentMarkupDto>.Success(
            new GatewayPaymentMarkupDto(result.Value!.Markup, result.Value.Provider));
    }

    /// <summary>
    /// Bankadan dönen callback'i işler:
    /// 1. Ödeme kaydını bulur
    /// 2. Banka yanıtını doğrular
    /// 3. Payment durumunu Completed/Failed yapar
    /// </summary>
    public async Task<Result<GatewayCallbackResultDto>> Handle(
        ProcessGatewayCallbackCommand req, CancellationToken ct)
    {
        var payment = await _repo.GetByIdAsync(req.PaymentId, ct);
        if (payment is null)
            return Result<GatewayCallbackResultDto>.Failure("Ödeme kaydı bulunamadı.");

        var result = await _gatewayService.ProcessCallbackAsync(req.Parameters, ct);
        if (!result.IsSuccess)
            return Result<GatewayCallbackResultDto>.Failure(result.Error!);

        if (result.Value!.Success)
        {
            payment.Complete(result.Value.TransactionId);
            await _orderStatusService.MarkOrderAsCompletedAsync(payment.OrderId, ct);
        }
        else
        {
            payment.Fail();
        }

        _repo.Update(payment);
        await _repo.SaveChangesAsync(ct);

        return Result<GatewayCallbackResultDto>.Success(new GatewayCallbackResultDto(
            result.Value.Success,
            result.Value.TransactionId,
            result.Value.AuthCode,
            result.Value.ResponseMessage,
            result.Value.TotalAmount));
    }
}

