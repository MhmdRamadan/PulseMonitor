using MediatR;
using PulseMonitor.Application.Common.Models;

namespace PulseMonitor.Application.Features.Reports.Commands.CreateReport;

public record CreateReportCommand(Guid ServerId, DateTime FromUtc, DateTime ToUtc) : IRequest<Result<ReportDto>>;

public record ReportDto(Guid Id, string Status, DateTime CreatedAtUtc);
