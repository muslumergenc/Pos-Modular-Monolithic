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

public interface ITableService
{
    Task<int?> GetTableNumberAsync(Guid tableId, CancellationToken ct = default);
    Task SetTableOccupiedAsync(Guid tableId, CancellationToken ct = default);
    Task SetTableAvailableAsync(Guid tableId, CancellationToken ct = default);
}

