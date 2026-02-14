using Microsoft.Extensions.Logging;

namespace PulseMonitor.Infrastructure.BackgroundJobs;

public class MaintenanceJob
{
    private readonly ILogger<MaintenanceJob> _logger;

    public MaintenanceJob(ILogger<MaintenanceJob> logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Maintenance job executed at {Time}", DateTime.UtcNow);
        return Task.CompletedTask;
    }
}
