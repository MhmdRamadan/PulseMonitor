using MediatR;
using PulseMonitor.Application.Common.Models;

namespace PulseMonitor.Application.Features.Alerts.Queries.GetAlerts;

public record GetAlertsQuery(
    Guid? ServerId,
    int PageNumber = 1,
    int PageSize = 50
) : IRequest<PagedResult<AlertDto>>;

public record AlertDto(
    Guid Id,
    Guid ServerId,
    string ServerName,
    string Message,
    string Severity,
    double? ThresholdValue,
    DateTime TriggeredAtUtc,
    DateTime? AcknowledgedAtUtc
);
