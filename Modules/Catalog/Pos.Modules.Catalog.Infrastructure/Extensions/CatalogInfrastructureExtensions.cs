using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pos.Modules.Catalog.Application.Extensions;
using Pos.Modules.Catalog.Application.Interfaces;
using Pos.Modules.Catalog.Infrastructure.Persistence;
using Pos.Modules.Catalog.Infrastructure.Repositories;

namespace Pos.Modules.Catalog.Infrastructure.Extensions;

public static class CatalogInfrastructureExtensions
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddCatalogApplication();
        return services;
    }
}

