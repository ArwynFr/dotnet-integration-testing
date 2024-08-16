using ArwynFr.IntegrationTesting.Tests.Target;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests;

public class OtelTests(ITestOutputHelper output) : IntegrationTestBase<Program>(output)
{
    [Fact]
    public async Task OTEL_activties_captured()
    {
        await Client.GetAsync("/otel");
        Activities.Any(activity => activity.DisplayName == ActivityHelper.ActivityName).Should().BeTrue();
    }

    protected override string[] OpenTelemetrySourceNames => [ActivityHelper.Source.Name];
}
