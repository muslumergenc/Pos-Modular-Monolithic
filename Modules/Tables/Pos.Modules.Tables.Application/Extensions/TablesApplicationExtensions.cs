using Microsoft.Extensions.DependencyInjection;

namespace Pos.Modules.Tables.Application.Extensions;

public static class TablesApplicationExtensions
{
    public static IServiceCollection AddTablesApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TablesApplicationExtensions).Assembly));
        return services;
    }
}
