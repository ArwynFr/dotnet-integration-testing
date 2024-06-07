# ArwynFr.IntegrationTesting

This library provides utility classes for writing integration tests in
dotnet using `XUnit` and `WebApplicationFactory`.

[![Nuget.org](https://img.shields.io/nuget/v/ArwynFr.IntegrationTesting?style=for-the-badge)](https://www.nuget.org/packages/ArwynFr.IntegrationTesting/)
[![Nuget.org](https://img.shields.io/nuget/dt/ArwynFr.IntegrationTesting?style=for-the-badge)](https://www.nuget.org/packages/ArwynFr.IntegrationTesting/)
[![GitHub
License](https://img.shields.io/github/license/ArwynFr/dotnet-integration-testing?style=for-the-badge)](https://github.com/ArwynFr/dotnet-integration-testing#MIT-1-ov-file)

## Installation

```shell
dotnet new classlib -n MyTestProject
```

```shell
dotnet add package ArwynFr.IntegrationTesting
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package xunit.runner.visualstudio
```

## Usage

Read [advanced usage
documentation](https://github.com/ArwynFr/dotnet-integration-testing/blob/main/.github/USAGE.md)
for further details.

By default, the lib redirects the tested application logs to XUnit
output, so you get them in the test output in case of failure. It also
overwrites the application configuration with values from user secrets
and environement variables.

```cs
public class MyTest(ITestOutputHelper output) : IntegrationTestBase<Program>(output)
{
    // Override a service with fake implementation in the tested app
    protected override void ConfigureAppServices(IServiceCollection services)
        => services.AddSingleton<IMyService, FakeService>();

    [Fact]
    public async Task OnTest()
    {
        // Call system under test
        var response = await Client.GetFromJsonAsync<OrderDto>($"/order");

        response.Should().HaveValue();
    }
}
```

### EntityFrameworkCore integration

If your application uses EFcore, add the DbContext as a generic parameter
and provide a configuration method. You can override the DbContext lifetime
strategy according to your needs:

```cs
public class TestBaseDb(ITestOutputHelper output) : IntegrationTestBase<Program, MyDbContext>(output)
{
    // Create and drop a database for every test execution
    protected override IDatabaseTestStrategy<Context> DatabaseTestStrategy
        => IDatabaseTestStrategy<MyDbContext>.DatabasePerTest;

    // Configure EFcore with a random database name par test
    protected override void ConfigureDbContext(DbContextOptionsBuilder builder)
        => builder.UseSqlite($"Data Source={Guid.NewGuid()}.sqlite");

    [Fact]
    public async Task OnTest()
    {
        // Access the injected dbcontext
        var value = await Database.Values
            .Where(val => val.Id == 1)
            .Select(val => val.Result)
            .FirstOrDefaultAsync();

        // Call system under test
        var result = await Client.GetFromJsonAsync<int>("/api/value/1");

        result.Should().Be(value + 1);
    }
}
```

### OpenTelemetry integration

You can access OTEL activities produced by your application in your tests.
This requires you to list which activities to monitor:

```cs
public class OpenTelemetryTests(ITestOutputHelper output) : IntegrationTestBase<Program>(output)
{
    // Tell the library which OTEL sources you want to monitor
    // Traces from other sources will be ignored
    protected override string[] OpenTelemetrySourceNames => ["SourceA", "SourceB"];

    [Fact]
    public async Task Otel()
    {
        // Call system under test
        await Client.GetAsync("/otel");

        // Assert on the collection of all activities collected
        Activities.Any(activity => activity.DisplayName == "Hello").Should().BeTrue();
    }
}
```

## Contributing

This project welcomes contributions:

**Request for support:**  
We do not provide commercial support for this product.

**Disclose vulnerability:**  
[Read our security policy](https://github.com/ArwynFr/dotnet-integration-testing/blob/main/.github/SECURITY.md)  
[Create a new security advisory on GitHub](https://github.com/ArwynFr/dotnet-integration-testing/security/advisories)

**Report malfunctions:**  
[Create a new issue on GitHub](https://github.com/ArwynFr/dotnet-integration-testing/issues/new/choose)

**Suggest a feature:**  
[Create a new issue on GitHub](https://github.com/ArwynFr/dotnet-integration-testing/issues/new/choose)

**Offer some code:**  
[Read our definition of done](https://github.com/ArwynFr/dotnet-integration-testing/blob/main/.github/CONTRIBUTING.md#definition-of-done)  
[Fork the repository](https://github.com/ArwynFr/dotnet-integration-testing/fork)  
[Submit a pull-request](https://github.com/ArwynFr/dotnet-integration-testing/compare)

**Moderate contributions:**  
[Read our governance policy](https://github.com/ArwynFr/dotnet-integration-testing/blob/main/.github/GOVERNANCE.md)  
This project is not currently appointing new moderators.
