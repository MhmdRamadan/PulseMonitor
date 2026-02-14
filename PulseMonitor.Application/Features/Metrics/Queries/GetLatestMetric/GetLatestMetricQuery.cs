using MediatR;
using PulseMonitor.Application.Features.Metrics.Queries.GetMetricsByServer;

namespace PulseMonitor.Application.Features.Metrics.Queries.GetLatestMetric;

public record GetLatestMetricQuery(Guid ServerId) : IRequest<MetricDto?>;
