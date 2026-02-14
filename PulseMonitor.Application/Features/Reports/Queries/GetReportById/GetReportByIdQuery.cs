using MediatR;

namespace PulseMonitor.Application.Features.Reports.Queries.GetReportById;

public record GetReportByIdQuery(Guid Id) : IRequest<ReportDetailDto?>;

public record ReportDetailDto(
    Guid Id,
    Guid ServerId,
    string ServerName,
    string Status,
    DateTime FromUtc,
    DateTime ToUtc,
    string? FilePath,
    DateTime CreatedAtUtc,
    DateTime? CompletedAtUtc,
    string? ErrorMessage
);
