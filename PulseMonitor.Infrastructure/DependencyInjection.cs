using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Infrastructure.BackgroundJobs;
using PulseMonitor.Infrastructure.Persistence;
using PulseMonitor.Infrastructure.Services;
using Hangfire;
using Hangfire.SqlServer;

namespace PulseMonitor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=PulseMonitor;Trusted_Connection=True;";
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseSqlServer(conn));
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IReportJobService, ReportJobService>();
        services.AddSingleton<Application.Common.Interfaces.ISystemMetricsProvider, SystemMetricsService>();
        services.AddScoped<MetricsCollectionJob>();
        services.AddScoped<ReportGenerationJob>();
        services.AddScoped<MaintenanceJob>();
        services.AddScoped<ReportCompletedContinuationJob>();

        services.AddAuthentication()
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "PulseMonitor",
                    ValidAudience = configuration["Jwt:Audience"] ?? "PulseMonitor",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "PulseMonitorSecretKeyAtLeast32CharactersLong!!")),
                    ClockSkew = TimeSpan.FromMinutes(2)
                };
                opts.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        var path = ctx.Request.Path;
                        if (path.StartsWithSegments("/hubs"))
                        {
                            var token = ctx.Request.Query["access_token"].FirstOrDefault()
                                ?? ctx.Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "");
                            if (!string.IsNullOrEmpty(token))
                                ctx.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddHangfire(c => c.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(conn, new SqlServerStorageOptions { SchemaName = "Hangfire" }));
        services.AddHangfireServer();

        return services;
    }
}
