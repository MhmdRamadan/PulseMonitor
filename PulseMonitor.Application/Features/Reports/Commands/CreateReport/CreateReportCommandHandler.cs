using MediatR;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Application.Common.Models;
using PulseMonitor.Domain.Entities;
using PulseMonitor.Domain.Enums;
using PulseMonitor.Domain.Exceptions;

namespace PulseMonitor.Application.Features.Reports.Commands.CreateReport;

public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, Result<ReportDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IReportJobService _reportJob;

    public CreateReportCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IReportJobService reportJob)
    {
        _db = db;
        _currentUser = currentUser;
        _reportJob = reportJob;
    }

    public async Task<Result<ReportDto>> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
            return Result<ReportDto>.Failure("User not authenticated.");

        var userId = Guid.Parse(_currentUser.UserId);
        var server = await _db.Servers.FindAsync(new object[] { request.ServerId }, cancellationToken);
        if (server == null)
            return Result<ReportDto>.Failure("Server not found.");

        var report = new Report
        {
            ServerId = request.ServerId,
            RequestedByUserId = userId,
            Status = ReportStatus.Pending,
            FromUtc = request.FromUtc,
            ToUtc = request.ToUtc,
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.Reports.Add(report);
        await _db.SaveChangesAsync(cancellationToken);

        _reportJob.EnqueueReportGeneration(report.Id);

        return Result<ReportDto>.Success(new ReportDto(report.Id, report.Status.ToString(), report.CreatedAtUtc));
    }
}
