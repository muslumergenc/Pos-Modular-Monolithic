using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Pos.Modules.Payments.Application.Extensions;

public static class PaymentsApplicationExtensions
{
    public static IServiceCollection AddPaymentsApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PaymentsApplicationExtensions).Assembly));
        services.AddValidatorsFromAssembly(typeof(PaymentsApplicationExtensions).Assembly);
        return services;
    }
}

