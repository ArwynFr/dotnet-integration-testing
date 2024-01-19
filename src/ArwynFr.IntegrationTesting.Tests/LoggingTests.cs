﻿using FluentAssertions;

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ArwynFr.IntegrationTesting.Tests;

// Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor.
// The value might be captured by the base class as well.
// Reason: used to introspect the helper because we are testing the library
#pragma warning disable CS9107
public class LoggingTests(ITestOutputHelper output) : IntegrationTestBase<Program>(output)
#pragma warning restore CS9107
{

    [Fact]
    public async Task EmptyTest_ShouldWriteToXunit()
    {
        await Client.GetAsync("/");
        output.Should().BeOfType<TestOutputHelper>()
           .Subject.Output.Should().NotBeNullOrEmpty();
    }
}