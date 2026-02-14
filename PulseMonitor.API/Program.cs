using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using PulseMonitor.API.Hubs;
using PulseMonitor.API.Middleware;
using PulseMonitor.API.Services;
using PulseMonitor.Application;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Infrastructure;
using PulseMonitor.Infrastructure.BackgroundJobs;
using PulseMonitor.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Logging
// --------------------
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
    cfg.WriteTo.Console();
});

// --------------------
// Add Services
// --------------------
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// HttpContext access & custom services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IMetricPushService, SignalRMetricPushService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SignalR
builder.Services.AddSignalR();

// Cors (support UI on 3000 and API origins so token is accepted)
var origins = (builder.Configuration["Cors:Origins"] ?? "http://localhost:3000,https://localhost:3000,https://localhost:7166,http://localhost:5283")
    .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithExposedHeaders("Content-Disposition");
    });
});

// Hangfire
builder.Services.AddHangfire(cfg =>
{
    cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
       .UseSimpleAssemblyNameTypeSerializer()
       .UseRecommendedSerializerSettings()
       .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfireServer();

var app = builder.Build();

// --------------------
// Middleware
// --------------------
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

// --------------------
// Swagger
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --------------------
// Map Endpoints
// --------------------
app.MapControllers();
app.MapHub<MonitoringHub>("/hubs/monitoring");

// --------------------
// Hangfire Dashboard
// --------------------
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter(builder.Environment.IsDevelopment()) }
});

// --------------------
// Database Migration & Seed
// --------------------
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await DataSeeder.SeedAsync(db, scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>());
}

// --------------------
// Recurring & Scheduled Jobs
// --------------------
RecurringJob.AddOrUpdate<MetricsCollectionJob>(
    "collect-metrics",
    x => x.ExecuteAsync(CancellationToken.None),
    "*/2 * * * *" // every 2 minutes
);

BackgroundJob.Schedule<MaintenanceJob>(
    x => x.ExecuteAsync(CancellationToken.None),
    TimeSpan.FromMinutes(5)
);

app.Run();


// --------------------
// Hangfire Authorization Filter
// --------------------
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly bool _allowAccess;

    public HangfireAuthorizationFilter(bool allowAccess)
    {
        _allowAccess = allowAccess;
    }

    public bool Authorize(DashboardContext context)
    {
        return _allowAccess;
    }
}
