using FluentValidation;

namespace PulseMonitor.Application.Features.Reports.Commands.CreateReport;

public class CreateReportCommandValidator : AbstractValidator<CreateReportCommand>
{
    public CreateReportCommandValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();
        RuleFor(x => x.FromUtc).LessThanOrEqualTo(x => x.ToUtc);
        RuleFor(x => x.ToUtc).GreaterThanOrEqualTo(x => x.FromUtc);
    }
}
