using Hangfire;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Infrastructure.BackgroundJobs;

namespace PulseMonitor.Infrastructure.Services;

public class ReportJobService : IReportJobService
{
    public string EnqueueReportGeneration(Guid reportId)
    {
        var jobId = BackgroundJob.Enqueue<ReportGenerationJob>(x => x.ExecuteAsync(reportId, CancellationToken.None));
        BackgroundJob.ContinueJobWith<ReportCompletedContinuationJob>(jobId, x => x.ExecuteAsync(reportId, "Completed", CancellationToken.None));
        return jobId;
    }
}
