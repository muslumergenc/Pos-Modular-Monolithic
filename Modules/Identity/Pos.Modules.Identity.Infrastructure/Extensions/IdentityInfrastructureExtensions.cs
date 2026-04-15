using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pos.Modules.Identity.Application.Extensions;
using Pos.Modules.Identity.Application.Interfaces;
using Pos.Modules.Identity.Domain.Entities;
using Pos.Modules.Identity.Infrastructure.Persistence;
using Pos.Modules.Identity.Infrastructure.Services;

namespace Pos.Modules.Identity.Infrastructure.Extensions;

public static class IdentityInfrastructureExtensions
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<IdentityDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddIdentityApplication();
        return services;
    }
}

