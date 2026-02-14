using MediatR;
using PulseMonitor.Application.Common.Models;

namespace PulseMonitor.Application.Features.Reports.Queries.GetReports;

public record GetReportsQuery(
    Guid? ServerId,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<PagedResult<ReportListItemDto>>;

public record ReportListItemDto(
    Guid Id,
    Guid ServerId,
    string ServerName,
    string Status,
    DateTime FromUtc,
    DateTime ToUtc,
    DateTime CreatedAtUtc,
    DateTime? CompletedAtUtc
);
