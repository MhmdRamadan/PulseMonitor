using MediatR;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Domain.Entities;
using PulseMonitor.Domain.Exceptions;

namespace PulseMonitor.Application.Features.AlertRules.Commands.CreateAlertRule;

public class CreateAlertRuleCommandHandler : IRequestHandler<CreateAlertRuleCommand, Guid>
{
    private readonly IApplicationDbContext _db;

    public CreateAlertRuleCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateAlertRuleCommand request, CancellationToken cancellationToken)
    {
        var server = await _db.Servers.FindAsync(new object[] { request.ServerId }, cancellationToken)
            ?? throw new NotFoundException(nameof(Server), request.ServerId);
        var rule = new AlertRule
        {
            ServerId = request.ServerId,
            MetricType = request.MetricType,
            Operator = request.Operator,
            ThresholdValue = request.ThresholdValue,
            Severity = request.Severity,
            IsActive = true
        };
        _db.AlertRules.Add(rule);
        await _db.SaveChangesAsync(cancellationToken);
        return rule.Id;
    }
}
