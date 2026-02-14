using MediatR;

namespace PulseMonitor.Application.Features.AlertRules.Queries.GetAlertRulesByServer;

public record GetAlertRulesByServerQuery(Guid ServerId) : IRequest<IReadOnlyList<AlertRuleDto>>;

public record AlertRuleDto(
    Guid Id,
    Guid ServerId,
    string MetricType,
    string Operator,
    double ThresholdValue,
    string Severity,
    bool IsActive
);
