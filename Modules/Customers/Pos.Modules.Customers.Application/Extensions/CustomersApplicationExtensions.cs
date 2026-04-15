using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Pos.Modules.Customers.Application.Extensions;

public static class CustomersApplicationExtensions
{
    public static IServiceCollection AddCustomersApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CustomersApplicationExtensions).Assembly));
        services.AddValidatorsFromAssembly(typeof(CustomersApplicationExtensions).Assembly);
        return services;
    }
}

