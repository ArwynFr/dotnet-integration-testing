# ArwynFr.IntegrationTesting

This library provides utility classes for writing integration tests in dotnet using `XUnit` and `WebApplicationFactory`.

## Installing

```
> dotnet add package ArwynFr.IntegrationTesting
```

## Usage

See [advanced usage guide](USAGE.adoc) for details.

By default, the lib redirects the tested application logs to XUnit output, so you get them in the test output in case of failure. It also overwrites the application configuration with values from user secrets and environement variables.

```cs
public class MyTest : IntegrationTestBase<Program>
{
    public MyTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task OnTest()
    {
        var response = await Client.GetFromJsonAsync<OrderDto>($"/order");

        response.Should().HaveValue();
    }
    
    // Override a service with fake implementation in the tested app
    protected override void ConfigureAppServices(IServiceCollection services)
        => services.AddSingleton<IMyService, FakeService>();
}
```

## EntityFrameworkCore integration

```cs
public class TestBaseDb : IntegrationTestBase<Program, MyDbContext>
{
    public TestBaseDb(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task OnTest()
    {
        // Access the injected dbcontext
        var value = await Database.Values
            .Where(val => val.Id = 1)
            .Select(val => val.Result)
            .FirstOrDefaultAsync();

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