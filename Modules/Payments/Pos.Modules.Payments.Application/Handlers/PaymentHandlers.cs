using MediatR;
using Pos.Modules.Payments.Application.Commands;
using Pos.Modules.Payments.Application.DTOs;
using Pos.Modules.Payments.Application.Interfaces;
using Pos.Modules.Payments.Application.Queries;
using Pos.Modules.Payments.Domain.Entities;
using Pos.Shared.Common;

namespace Pos.Modules.Payments.Application.Handlers;

public class PaymentCommandHandler :
    IRequestHandler<ProcessPaymentCommand, Result<PaymentDto>>,
    IRequestHandler<RefundPaymentCommand, Result<PaymentDto>>
{
    private readonly IPaymentRepository _repo;
    private readonly IOrderStatusService _orderStatusService;

    public PaymentCommandHandler(IPaymentRepository repo, IOrderStatusService orderStatusService)
    {
        _repo = repo;
        _orderStatusService = orderStatusService;
    }

    public async Task<Result<PaymentDto>> Handle(ProcessPaymentCommand req, CancellationToken ct)
    {
        var orderInfo = await _orderStatusService.GetOrderInfoAsync(req.OrderId, ct);
        if (orderInfo is null) return Result<PaymentDto>.Failure("Sipariş bulunamadı.");
        if (orderInfo.Value.TotalAmount != req.Amount)
            return Result<PaymentDto>.Failure($"Ödeme tutarı sipariş tutarıyla eşleşmiyor. Beklenen: {orderInfo.Value.TotalAmount:C}");

        var payment = Payment.Create(req.OrderId, req.Amount, req.Method, req.Notes);

        // Ödeme işlemi simülasyonu — gerçek sistemde ödeme gateway çağırılır
        try
        {
            payment.Complete(transactionReference: $"TXN-{Guid.NewGuid():N}".ToUpper()[..16]);
            await _orderStatusService.MarkOrderAsCompletedAsync(req.OrderId, ct);
        }
        catch
        {
            payment.Fail();
        }

        await _repo.AddAsync(payment, ct);
        await _repo.SaveChangesAsync(ct);
        return Result<PaymentDto>.Success(Map(payment));
    }

    public async Task<Result<PaymentDto>> Handle(RefundPaymentCommand req, CancellationToken ct)
    {
        var payment = await _repo.GetByIdAsync(req.PaymentId, ct);
        if (payment is null) return Result<PaymentDto>.Failure("Ödeme kaydı bulunamadı.");
        payment.Refund();
        _repo.Update(payment);
        await _repo.SaveChangesAsync(ct);
        return Result<PaymentDto>.Success(Map(payment));
    }

    private static PaymentDto Map(Payment p) =>
        new(p.Id, p.OrderId, p.Amount, p.Method, p.Status, p.TransactionReference, p.Notes, p.ProcessedAt, p.CreatedAt);
}

public class PaymentQueryHandler :
    IRequestHandler<GetAllPaymentsQuery, Result<IEnumerable<PaymentDto>>>,
    IRequestHandler<GetPaymentByIdQuery, Result<PaymentDto>>
{
    private readonly IPaymentRepository _repo;
    public PaymentQueryHandler(IPaymentRepository repo) => _repo = repo;

    public async Task<Result<IEnumerable<PaymentDto>>> Handle(GetAllPaymentsQuery req, CancellationToken ct)
    {
        var list = req.OrderId.HasValue
            ? await _repo.FindAsync(p => p.OrderId == req.OrderId.Value, ct)
            : await _repo.GetAllAsync(ct);
        return Result<IEnumerable<PaymentDto>>.Success(list.Select(Map));
    }

    public async Task<Result<PaymentDto>> Handle(GetPaymentByIdQuery req, CancellationToken ct)
    {
        var p = await _repo.GetByIdAsync(req.Id, ct);
        if (p is null) return Result<PaymentDto>.Failure("Ödeme bulunamadı.");
        return Result<PaymentDto>.Success(Map(p));
    }

    private static PaymentDto Map(Payment p) =>
        new(p.Id, p.OrderId, p.Amount, p.Method, p.Status, p.TransactionReference, p.Notes, p.ProcessedAt, p.CreatedAt);
}

