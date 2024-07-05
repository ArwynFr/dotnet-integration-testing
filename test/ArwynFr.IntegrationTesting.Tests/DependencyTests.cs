using ArwynFr.IntegrationTesting.Tests.Target;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests;

public class DependencyTests(ITestOutputHelper output) : IntegrationTestBase<Program>(output)
{
    [Fact]
    public void Injected_service_accessible_in_test()
    {
        Services.GetService<IDummyService>().Should().BeOfType<OverrideDummyService>();
    }

    [Fact]
    public async Task Injected_service_used_in_SUT()
    {
        var expected = Services.GetRequiredService<IDummyService>().GetHashCode();
        var actual = await Client.GetFromJsonAsync<int>("/service");
        actual.Should().Be(expected);
    }

    protected override void ConfigureAppServices(IServiceCollection services)
        => services.AddSingleton<IDummyService, OverrideDummyService>();

    private class OverrideDummyService : IDummyService;
}
