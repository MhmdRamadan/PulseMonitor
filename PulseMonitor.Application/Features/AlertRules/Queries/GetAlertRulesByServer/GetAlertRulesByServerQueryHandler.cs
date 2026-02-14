using MediatR;
using Microsoft.EntityFrameworkCore;
using PulseMonitor.Application.Common.Interfaces;

namespace PulseMonitor.Application.Features.AlertRules.Queries.GetAlertRulesByServer;

public class GetAlertRulesByServerQueryHandler : IRequestHandler<GetAlertRulesByServerQuery, IReadOnlyList<AlertRuleDto>>
{
    private readonly IApplicationDbContext _db;

    public GetAlertRulesByServerQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<AlertRuleDto>> Handle(GetAlertRulesByServerQuery request, CancellationToken cancellationToken)
    {
        return await _db.AlertRules
            .AsNoTracking()
            .Where(r => r.ServerId == request.ServerId)
            .Select(r => new AlertRuleDto(
                r.Id,
                r.ServerId,
                r.MetricType.ToString(),
                r.Operator,
                r.ThresholdValue,
                r.Severity.ToString(),
                r.IsActive))
            .ToListAsync(cancellationToken);
    }
}
