using Microsoft.EntityFrameworkCore;
using Pos.Modules.Orders.Domain.Enums;
using Pos.Modules.Orders.Infrastructure.Persistence;
using Pos.Modules.Payments.Application.Interfaces;

namespace Pos.Modules.Payments.Infrastructure.Services;

public class OrderStatusService : IOrderStatusService
{
    private readonly OrdersDbContext _ordersDb;
    public OrderStatusService(OrdersDbContext ordersDb) => _ordersDb = ordersDb;

    public async Task<(decimal TotalAmount, bool Exists)?> GetOrderInfoAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await _ordersDb.Orders.FirstOrDefaultAsync(o => o.Id == orderId, ct);
        if (order is null) return null;
        return (order.TotalAmount, true);
    }

    public async Task MarkOrderAsCompletedAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await _ordersDb.Orders.FirstOrDefaultAsync(o => o.Id == orderId, ct);
        if (order is not null)
        {
            order.UpdateStatus(OrderStatus.Completed);
            await _ordersDb.SaveChangesAsync(ct);
        }
    }
}

