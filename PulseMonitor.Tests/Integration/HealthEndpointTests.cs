using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace PulseMonitor.Tests.Integration;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<global::Program>>
{
    private readonly WebApplicationFactory<global::Program> _factory;

    public HealthEndpointTests(WebApplicationFactory<global::Program> factory)
    {
        _factory = factory;
    }

    [Fact(Skip = "Swashbuckle/OpenApi type load issue with .NET 10 in test host. Run API and GET /api/v1/health manually.")]
    public async Task Health_ReturnsOkOrUnhealthy()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/v1/health");
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == (HttpStatusCode)503);
    }
}
