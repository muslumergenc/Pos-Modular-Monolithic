using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pos.Modules.Orders.Application.Extensions;
using Pos.Modules.Orders.Application.Interfaces;
using Pos.Modules.Orders.Infrastructure.Persistence;
using Pos.Modules.Orders.Infrastructure.Repositories;
using Pos.Modules.Orders.Infrastructure.Services;

namespace Pos.Modules.Orders.Infrastructure.Extensions;

public static class OrdersInfrastructureExtensions
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddOrdersApplication();
        return services;
    }
}

