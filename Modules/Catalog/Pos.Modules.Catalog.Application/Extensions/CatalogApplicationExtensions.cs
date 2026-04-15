using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Pos.Modules.Catalog.Application.Extensions;

public static class CatalogApplicationExtensions
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CatalogApplicationExtensions).Assembly));
        services.AddValidatorsFromAssembly(typeof(CatalogApplicationExtensions).Assembly);
        return services;
    }
}

