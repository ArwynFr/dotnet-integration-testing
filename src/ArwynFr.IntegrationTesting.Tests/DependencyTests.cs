using System.Net.Http.Json;

using ArwynFr.IntegrationTesting.Tests.Target;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests;

public class DependencyTests(ITestOutputHelper output) : IntegrationTestBase<Program>(output)
{
    [Fact]
    public void InjectedServiceAvailableInTest()
    {
        Services.GetService<IDummyService>().Should().BeOfType<OverrideDummyService>();
    }

    [Fact]
    public async Task InjectedServiceAvailableInApp()
    {
        var expected = Services.GetRequiredService<IDummyService>().GetHashCode();
        var actual = await Client.GetFromJsonAsync<int>("/service");
        actual.Should().Be(expected);
    }

    protected override void ConfigureAppServices(IServiceCollection services)
        => services.AddSingleton<IDummyService, OverrideDummyService>();

    private class OverrideDummyService : IDummyService;
}
