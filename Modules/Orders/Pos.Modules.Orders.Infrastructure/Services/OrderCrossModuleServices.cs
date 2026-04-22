using Microsoft.EntityFrameworkCore;
using Pos.Modules.Catalog.Infrastructure.Persistence;
using Pos.Modules.Customers.Infrastructure.Persistence;
using Pos.Modules.Orders.Application.Interfaces;
using Pos.Modules.Tables.Infrastructure.Persistence;
using Pos.Modules.Tables.Domain.Entities;

namespace Pos.Modules.Orders.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly CatalogDbContext _catalogDb;
    public ProductService(CatalogDbContext catalogDb) => _catalogDb = catalogDb;

    public async Task<(string Name, decimal Price, int Stock)?> GetProductAsync(Guid productId, CancellationToken ct = default)
    {
        var product = await _catalogDb.Products.FindAsync(new object[] { productId }, ct);
        if (product is null) return null;
        return (product.Name, product.Price, product.Stock);
    }

    public async Task DecreaseStockAsync(Guid productId, int quantity, CancellationToken ct = default)
    {
        var product = await _catalogDb.Products.FindAsync(new object[] { productId }, ct);
        if (product is not null)
        {
            product.DecreaseStock(quantity);
            await _catalogDb.SaveChangesAsync(ct);
        }
    }
}

public class CustomerService : ICustomerService
{
    private readonly CustomersDbContext _customersDb;
    public CustomerService(CustomersDbContext customersDb) => _customersDb = customersDb;

    public async Task<string?> GetCustomerNameAsync(Guid customerId, CancellationToken ct = default)
    {
        var customer = await _customersDb.Customers.FindAsync(new object[] { customerId }, ct);
        return customer?.FullName;
    }
}

public class TableService : ITableService
{
    private readonly TablesDbContext _tablesDb;
    public TableService(TablesDbContext tablesDb) => _tablesDb = tablesDb;

    public async Task<int?> GetTableNumberAsync(Guid tableId, CancellationToken ct = default)
    {
        var table = await _tablesDb.Tables.FindAsync(new object[] { tableId }, ct);
        return table?.Number;
    }

    public async Task SetTableOccupiedAsync(Guid tableId, CancellationToken ct = default)
    {
        var table = await _tablesDb.Tables.FindAsync(new object[] { tableId }, ct);
        if (table is not null) { table.Close(); await _tablesDb.SaveChangesAsync(ct); }
    }

    public async Task SetTableAvailableAsync(Guid tableId, CancellationToken ct = default)
    {
        var table = await _tablesDb.Tables.FindAsync(new object[] { tableId }, ct);
        if (table is not null) { table.Open(); await _tablesDb.SaveChangesAsync(ct); }
    }
}


