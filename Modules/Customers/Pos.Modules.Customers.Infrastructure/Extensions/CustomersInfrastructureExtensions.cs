using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pos.Modules.Customers.Application.Extensions;
using Pos.Modules.Customers.Application.Interfaces;
using Pos.Modules.Customers.Infrastructure.Persistence;
using Pos.Modules.Customers.Infrastructure.Repositories;

namespace Pos.Modules.Customers.Infrastructure.Extensions;

public static class CustomersInfrastructureExtensions
{
    public static IServiceCollection AddCustomersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CustomersDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddCustomersApplication();
        return services;
    }
}

