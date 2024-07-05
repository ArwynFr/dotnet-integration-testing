using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests;

public class EmptyTests(ITestOutputHelper output) : IntegrationTestBase<Program>(output)
{
    [Fact]
    public void Calling_client_no_exception() => Client.Should().NotBeNull();
}
