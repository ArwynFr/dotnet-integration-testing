# ArwynFr.IntegrationTesting

This library provides utility classes for writing integration tests in
dotnet using `XUnit` and `WebApplicationFactory`.

[![Nuget.org](https://img.shields.io/nuget/v/ArwynFr.IntegrationTesting?style=for-the-badge)](https://www.nuget.org/packages/ArwynFr.IntegrationTesting/)
[![Nuget.org](https://img.shields.io/nuget/dt/ArwynFr.IntegrationTesting?style=for-the-badge)](https://www.nuget.org/packages/ArwynFr.IntegrationTesting/)
[![GitHub
License](https://img.shields.io/github/license/ArwynFr/dotnet-integration-testing?style=for-the-badge)](https://github.com/ArwynFr/dotnet-integration-testing#MIT-1-ov-file)

## Installation

    dotnet add package ArwynFr.IntegrationTesting

## Usage

Read [advanced usage
documentation](https://github.com/ArwynFr/dotnet-integration-testing/blob/main/.github/USAGE.md)
for further details.

By default, the lib redirects the tested application logs to XUnit
output, so you get them in the test output in case of failure. It also
overwrites the application configuration with values from user secrets
and environement variables.

```cs
public class MyTest : IntegrationTestBase<Program>
{
    public MyTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task OnTest()
    {
        // Call system under test
        var response = await Client.GetFromJsonAsync<OrderDto>($"/order");

        response.Should().HaveValue();
    }

    // Override a service with fake implementation in the tested app
    protected override void ConfigureAppServices(IServiceCollection services)
        => services.AddSingleton<IMyService, FakeService>();
}
```

### EntityFrameworkCore integration

```cs
public class TestBaseDb : IntegrationTestBase<Program, MyDbContext>
{
    public TestBaseDb(ITestOutputHelper output) : base(output) { }

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

    // Create and drop a database for every test execution
    protected override IDatabaseTestStrategy<Context> DatabaseTestStrategy
        => IDatabaseTestStrategy<MyDbContext>.DatabasePerTest;

    // Configure EFcore with a random database name
    protected override void ConfigureDbContext(DbContextOptionsBuilder builder)
        => builder.UseSqlite($"Data Source={Guid.NewGuid()}.sqlite");
}
```

### Fluent specification-based testing

First write a driver that tells the test framework how to execute AAA
operations. This class can implement multiple operations of the same kind:

```cs
public class SpecDriver(MyDbContext dbContext, HttpClient client) : TestDriverBase<SpecDriver>
{
    // Used to store the API call result
    private string? result;

    // Arrange
    public async Task ThereIsAnEntity(string name)
    {
        dbContext.Entities.Add(new Entity { Name = name });
        await dbContext.SaveChangesAsync();
    }

    // Another arrange
    public async Task ThereIsNoEntity(string name)
    {
        await dbContext.Entities.Where(entity => entity.Name == name).ExecuteDeleteAsync();
    }

    // Act
    public async Task FindEntityByName(string name)
    {
        result = await client.GetFromJsonAsync<string>($"/api/entities/{name}");
    }

    // Assert
    public Task ResultShouldBe(string name)
    {
        Assert.Equal(name, result);
        return Task.CompletedTask;
    }
}
```

Then write an integration test that injects the driver and use it to run
fluent specification-based tests:

```cs
public class SpecTest(ITestOutputHelper output) : TestBaseDb
{
    // Configure DI library
    protected override void ConfigureAppServices(IServiceCollection services)
    {
        base.ConfigureAppServices(services);
        services.AddSingleton(_ => new SpecDriver(Database, Client));
    }

    [Theory, InlineData("ArwynFr")]
    public async Task OnTest(string name)
    {
        await Services.GetRequiredService<SpecDriver>()
            .Given(x => x.ThereIsEntityWithName(name))
            .When(x => x.FindEntityWithName(name))
            .Then(x => x.ResultShouldBe(name))
            .ExecuteAsync();
    }
}
```

## Contributing

This project welcomes contributions:

**Request for support:**  
TBD

**Disclose vulnerability:**  
Please [create a new security advisory on GitHub](https://github.com/ArwynFr/dotnet-integration-testing/security/advisories)
\
[Read our security policy](https://github.com/ArwynFr/dotnet-integration-testing/blob/main/.github/SECURITY.md)

**Report malfunctions:**  
[Please create a new issue on GitHub](https://github.com/ArwynFr/dotnet-integration-testing/issues/new/choose)

**Suggest a feature:**  
[Please create a new issue on GitHub](https://github.com/ArwynFr/dotnet-integration-testing/issues/new/choose)

**Offer some code:**  
Please [fork the repository](https://github.com/ArwynFr/dotnet-integration-testing/fork)
and [submit a pull-request](https://github.com/ArwynFr/dotnet-integration-testing/compare)
\
[Read our definition of done in contributing guidelines](https://github.com/ArwynFr/dotnet-integration-testing/blob/main/.github/CONTRIBUTING.md)

**Moderate contributions:**  
This project is not currently appointing new moderators.
\
[Read our governance policy](https://github.com/ArwynFr/dotnet-integration-testing/blob/main/.github/GOVERNANCE.md)
