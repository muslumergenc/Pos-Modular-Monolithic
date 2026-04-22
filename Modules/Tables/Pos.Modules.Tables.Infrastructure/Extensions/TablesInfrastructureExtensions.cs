using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pos.Modules.Tables.Application.Extensions;
using Pos.Modules.Tables.Application.Interfaces;
using Pos.Modules.Tables.Infrastructure.Persistence;
using Pos.Modules.Tables.Infrastructure.Repositories;

namespace Pos.Modules.Tables.Infrastructure.Extensions;

public static class TablesInfrastructureExtensions
{
    public static IServiceCollection AddTablesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TablesDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<ITableRepository, TableRepository>();
        services.AddTablesApplication();
        return services;
    }
}
