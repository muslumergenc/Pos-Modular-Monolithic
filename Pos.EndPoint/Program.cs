using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pos.Modules.Catalog.Infrastructure.Extensions;
using Pos.Modules.Customers.Infrastructure.Extensions;
using Pos.Modules.Identity.Infrastructure.Extensions;
using Pos.Modules.Orders.Infrastructure.Extensions;
using Pos.Modules.Payments.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// ── Modüller ──────────────────────────────────────────────
builder.Services.AddIdentityModule(config);
builder.Services.AddCatalogModule(config);
builder.Services.AddCustomersModule(config);
builder.Services.AddOrdersModule(config);
builder.Services.AddPaymentsModule(config);

// ── Controllers (tüm modüllerin Presentation assembly'lerinden) ──
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Pos.Modules.Identity.Presentation.Controllers.AuthController).Assembly)
    .AddApplicationPart(typeof(Pos.Modules.Catalog.Presentation.Controllers.ProductsController).Assembly)
    .AddApplicationPart(typeof(Pos.Modules.Customers.Presentation.Controllers.CustomersController).Assembly)
    .AddApplicationPart(typeof(Pos.Modules.Orders.Presentation.Controllers.OrdersController).Assembly)
    .AddApplicationPart(typeof(Pos.Modules.Payments.Presentation.Controllers.PaymentsController).Assembly);

// ── JWT Authentication ──────────────────────────────────────
var jwtSettings = config.GetSection("JwtSettings");
var secret = jwtSettings["Secret"]!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
});
builder.Services.AddAuthorization();

// ── Swagger ─────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "POS API", Version = "v1", Description = "Modular Monolithic POS Payment System" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer", BearerFormat = "JWT", In = ParameterLocation.Header,
        Description = "JWT token giriniz: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// ── CORS (Pos.Web için) ──────────────────────────────────────
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// ── Veritabanı Migration ─────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await services.GetRequiredService<Pos.Modules.Identity.Infrastructure.Persistence.IdentityDbContext>().Database.MigrateAsync();
    await services.GetRequiredService<Pos.Modules.Catalog.Infrastructure.Persistence.CatalogDbContext>().Database.MigrateAsync();
    await services.GetRequiredService<Pos.Modules.Customers.Infrastructure.Persistence.CustomersDbContext>().Database.MigrateAsync();
    await services.GetRequiredService<Pos.Modules.Orders.Infrastructure.Persistence.OrdersDbContext>().Database.MigrateAsync();
    await services.GetRequiredService<Pos.Modules.Payments.Infrastructure.Persistence.PaymentsDbContext>().Database.MigrateAsync();
}

// ── Middleware Pipeline ──────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "POS API v1"));
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
