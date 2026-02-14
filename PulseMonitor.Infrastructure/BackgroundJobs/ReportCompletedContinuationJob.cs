using Microsoft.Extensions.Logging;

namespace PulseMonitor.Infrastructure.BackgroundJobs;

public class ReportCompletedContinuationJob
{
    private readonly ILogger<ReportCompletedContinuationJob> _logger;

    public ReportCompletedContinuationJob(ILogger<ReportCompletedContinuationJob> logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync(Guid reportId, string reportStatus, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Report {ReportId} completed with status {Status}. Continuation job executed.", reportId, reportStatus);
        return Task.CompletedTask;
    }
}
