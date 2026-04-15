using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Pos.Modules.Orders.Application.Extensions;

public static class OrdersApplicationExtensions
{
    public static IServiceCollection AddOrdersApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrdersApplicationExtensions).Assembly));
        services.AddValidatorsFromAssembly(typeof(OrdersApplicationExtensions).Assembly);
        return services;
    }
}

