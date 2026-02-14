using PulseMonitor.Application.Common.Models;
using Xunit;

namespace PulseMonitor.Tests.Application;

public class ResultTests
{
    [Fact]
    public void Success_ReturnsSucceededTrue()
    {
        var r = Result.Success();
        Assert.True(r.Succeeded);
        Assert.Empty(r.Errors);
    }

    [Fact]
    public void Failure_ReturnsSucceededFalseAndErrors()
    {
        var r = Result.Failure("Error1", "Error2");
        Assert.False(r.Succeeded);
        Assert.Equal(2, r.Errors.Length);
    }

    [Fact]
    public void ResultT_Success_ReturnsData()
    {
        var r = Result<int>.Success(42);
        Assert.True(r.Succeeded);
        Assert.Equal(42, r.Data);
    }

    [Fact]
    public void ResultT_Failure_ReturnsErrors()
    {
        var r = Result<int>.Failure("Fail");
        Assert.False(r.Succeeded);
        Assert.Single(r.Errors, "Fail");
    }
}
