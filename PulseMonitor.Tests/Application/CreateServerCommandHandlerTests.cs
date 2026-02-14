using Microsoft.EntityFrameworkCore;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Application.Features.Servers.Commands.CreateServer;
using PulseMonitor.Domain.Entities;
using PulseMonitor.Domain.Enums;
using PulseMonitor.Infrastructure.Persistence;
using Xunit;

namespace PulseMonitor.Tests.Application;

public class CreateServerCommandHandlerTests
{
    [Fact]
    public async Task Handle_AddsServerAndSaves()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        await using var db = new ApplicationDbContext(options);
        var handler = new CreateServerCommandHandler(db);

        var cmd = new CreateServerCommand("Test Server", "host01", "192.168.1.1", null);
        var id = await handler.Handle(cmd, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
        var added = await db.Servers.SingleAsync();
        Assert.Equal("Test Server", added.Name);
        Assert.Equal("host01", added.HostName);
        Assert.Equal("192.168.1.1", added.IpAddress);
        Assert.Equal(ServerStatus.Unknown, added.Status);
    }
}
