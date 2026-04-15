using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pos.Providers.GarantiBank;

public static class Startup
{
    public static IServiceCollection RegisterGarantiBank(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<GarantiBankOptions>(
            configuration.GetSection(GarantiBankOptions.SectionName));

        services.AddScoped<IGarantiBankPaymentProvider, GarantiBankPaymentProvider>();
        return services;
    }
}