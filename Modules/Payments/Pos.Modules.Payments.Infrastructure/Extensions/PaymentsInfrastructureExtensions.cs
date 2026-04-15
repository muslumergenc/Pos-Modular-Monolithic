using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pos.Modules.Payments.Application.Extensions;
using Pos.Modules.Payments.Application.Interfaces;
using Pos.Modules.Payments.Infrastructure.Persistence;
using Pos.Modules.Payments.Infrastructure.Repositories;
using Pos.Modules.Payments.Infrastructure.Services;
using Pos.Providers.GarantiBank;

namespace Pos.Modules.Payments.Infrastructure.Extensions;

public static class PaymentsInfrastructureExtensions
{
    public static IServiceCollection AddPaymentsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IOrderStatusService, OrderStatusService>();

        // Garanti Bankası 3D Secure ödeme gateway kaydı
        services.RegisterGarantiBank(configuration);
        services.AddScoped<IGatewayPaymentService, GatewayPaymentService>();

        services.AddPaymentsApplication();
        return services;
    }
}

