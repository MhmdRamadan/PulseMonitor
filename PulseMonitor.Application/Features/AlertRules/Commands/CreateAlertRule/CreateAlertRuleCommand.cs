using MediatR;
using PulseMonitor.Domain.Enums;

namespace PulseMonitor.Application.Features.AlertRules.Commands.CreateAlertRule;

public record CreateAlertRuleCommand(
    Guid ServerId,
    MetricType MetricType,
    string Operator,
    double ThresholdValue,
    AlertSeverity Severity
) : IRequest<Guid>;
