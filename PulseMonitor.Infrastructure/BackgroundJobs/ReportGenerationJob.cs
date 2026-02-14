using Microsoft.EntityFrameworkCore;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Domain.Enums;

namespace PulseMonitor.Infrastructure.BackgroundJobs;

public class ReportGenerationJob
{
    private readonly IApplicationDbContext _db;

    public ReportGenerationJob(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task ExecuteAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        var report = await _db.Reports
            .Include(r => r.Server)
            .FirstOrDefaultAsync(r => r.Id == reportId, cancellationToken);
        if (report == null || report.Status != ReportStatus.Pending) return;

        report.Status = ReportStatus.Processing;
        await _db.SaveChangesAsync(cancellationToken);

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken); // Simulate work
            var fileName = $"report_{reportId:N}_{DateTime.UtcNow:yyyyMMddHHmmss}.json";
            report.FilePath = $"reports/{fileName}";
            report.Status = ReportStatus.Completed;
            report.CompletedAtUtc = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            report.Status = ReportStatus.Failed;
            report.ErrorMessage = ex.Message;
            report.CompletedAtUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
