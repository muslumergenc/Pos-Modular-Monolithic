using Pos.Modules.Tables.Domain.Entities;
using Pos.Shared.Abstractions;

namespace Pos.Modules.Tables.Application.Interfaces;

public interface ITableRepository : IRepository<RestaurantTable>
{
    Task<RestaurantTable?> GetByNumberAsync(int number, CancellationToken ct = default);
}
