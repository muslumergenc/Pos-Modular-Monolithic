using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Pos.Modules.Identity.Application.Extensions;

public static class IdentityApplicationExtensions
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IdentityApplicationExtensions).Assembly));
        services.AddValidatorsFromAssembly(typeof(IdentityApplicationExtensions).Assembly);
        return services;
    }
}

