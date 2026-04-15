using Pos.Modules.Orders.Domain.Entities;
using Pos.Shared.Abstractions;

namespace Pos.Modules.Orders.Application.Interfaces;

public interface IOrderRepository : IRepository<Order> { }

public interface IProductService
{
    Task<(string Name, decimal Price, int Stock)?> GetProductAsync(Guid productId, CancellationToken ct = default);
    Task DecreaseStockAsync(Guid productId, int quantity, CancellationToken ct = default);
}

public interface ICustomerService
{
    Task<string?> GetCustomerNameAsync(Guid customerId, CancellationToken ct = default);
}

