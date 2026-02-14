using PulseMonitor.Application.Features.Servers.Commands.CreateServer;
using Xunit;

namespace PulseMonitor.Tests.Application;

public class CreateServerCommandValidatorTests
{
    private readonly CreateServerCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_When_NameEmpty()
    {
        var result = _validator.Validate(new CreateServerCommand("", "host", null, null));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateServerCommand.Name));
    }

    [Fact]
    public void Should_NotHaveError_When_Valid()
    {
        var result = _validator.Validate(new CreateServerCommand("My Server", "host01.internal", "10.0.0.1", "Desc"));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_HaveError_When_HostNameEmpty()
    {
        var result = _validator.Validate(new CreateServerCommand("Name", "", null, null));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateServerCommand.HostName));
    }
}
