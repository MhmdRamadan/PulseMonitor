using FluentValidation;
using PulseMonitor.Domain.Enums;

namespace PulseMonitor.Application.Features.AlertRules.Commands.CreateAlertRule;

public class CreateAlertRuleCommandValidator : AbstractValidator<CreateAlertRuleCommand>
{
    public CreateAlertRuleCommandValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();
        RuleFor(x => x.Operator).NotEmpty().Must(o => new[] { ">", ">=", "<", "<=" }.Contains(o));
        RuleFor(x => x.ThresholdValue).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ThresholdValue).LessThanOrEqualTo(100)
            .When(x => x.MetricType == MetricType.CpuUsage || x.MetricType == MetricType.MemoryUsage || x.MetricType == MetricType.DiskUsage);
    }
}
