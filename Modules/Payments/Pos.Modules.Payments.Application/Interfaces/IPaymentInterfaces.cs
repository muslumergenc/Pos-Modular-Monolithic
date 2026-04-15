using Pos.Modules.Payments.Domain.Entities;
using Pos.Shared.Abstractions;

namespace Pos.Modules.Payments.Application.Interfaces;

public interface IPaymentRepository : IRepository<Payment> { }

public interface IOrderStatusService
{
    Task<(decimal TotalAmount, bool Exists)?> GetOrderInfoAsync(Guid orderId, CancellationToken ct = default);
    Task MarkOrderAsCompletedAsync(Guid orderId, CancellationToken ct = default);
}

