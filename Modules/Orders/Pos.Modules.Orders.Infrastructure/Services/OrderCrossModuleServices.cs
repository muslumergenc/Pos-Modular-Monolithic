using Microsoft.EntityFrameworkCore;
using Pos.Modules.Catalog.Infrastructure.Persistence;
using Pos.Modules.Customers.Infrastructure.Persistence;
using Pos.Modules.Orders.Application.Interfaces;

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

