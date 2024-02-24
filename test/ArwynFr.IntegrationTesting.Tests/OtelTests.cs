using System.Text.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Builder;

using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests;

public class OtelTests(ITestOutputHelper output) : IntegrationTestBase<Program>(output)
{
  [Fact]
  public async Task Otel()
  {
    await Client.GetAsync("/otel");
    Activities.Any(activity => activity.DisplayName == ActivityHelper.ActivityName).Should().BeTrue();
  }

  [Fact]
  public async Task NoOtel()
  {
    await Client.GetAsync("/service");
    Activities.Any().Should().BeTrue();
  }

  protected override string[] OpenTelemetrySourceNames => [ActivityHelper.Source.Name];
}