using FluentValidation;

namespace PulseMonitor.Application.Features.Servers.Commands.UpdateServer;

public class UpdateServerCommandValidator : AbstractValidator<UpdateServerCommand>
{
    public UpdateServerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.HostName).NotEmpty().MaximumLength(500);
        RuleFor(x => x.IpAddress).MaximumLength(45).When(x => !string.IsNullOrEmpty(x.IpAddress));
        RuleFor(x => x.Description).MaximumLength(2000).When(x => x.Description != null);
    }
}
