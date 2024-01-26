using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests;

public class ConfigurationTests(ITestOutputHelper output) : IntegrationTestBase<Program>(output)
{
    // WARNING : configuration overrides are not unit testable
    // it would require to programatically set environment varaibles or user secrets before class instanciation
}