using MediatR;
using Microsoft.EntityFrameworkCore;
using PulseMonitor.Application.Common.Interfaces;

namespace PulseMonitor.Application.Features.Reports.Queries.GetReportById;

public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, ReportDetailDto?>
{
    private readonly IApplicationDbContext _db;

    public GetReportByIdQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ReportDetailDto?> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {
        var r = await _db.Reports
            .AsNoTracking()
            .Include(x => x.Server)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (r == null) return null;
        return new ReportDetailDto(
            r.Id,
            r.ServerId,
            r.Server.Name,
            r.Status.ToString(),
            r.FromUtc,
            r.ToUtc,
            r.FilePath,
            r.CreatedAtUtc,
            r.CompletedAtUtc,
            r.ErrorMessage
        );
    }
}
