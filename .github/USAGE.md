# Advanced usage documentation

This library uses `WebApplicationFactory` for integration testing.
Please read [Microsoft’s paper on integration
testing](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0)
for details on how this library works.

## Definitions

### Tested application

A dotnet application that you want to test. Also known as the *system
under test* (SUT).

### Entrypoint

A specific class of the tested application that is run when the
application start. For top-level statement applications this is the
`Program` class.

### Test project

A XUnit test project that implements tests and ensures that the tested
application behaves as expected.

## Basic integration test

Simply extend the `IntegrationTestBase<TEntryPoint>` class and provide
the entrypoint class you want to test:

```cs
public class MyTest(ITestOutputHelper output) : IntegrationTestBase<Program>(output)
{
}
```

## Arranging the tested application

You can override the following methods to customize the tested
application:

### `ConfigureAppConfiguration`

Change the sources for the app configuration. By default, overrides the
application configuration sources with the following (lower overrides
higher):

- `appsettings.json` file
- User secrets associated with the assembly that contains the entrypoint
- User secrets associated with the assembly that contains the test class
- Environment variables

### `ConfigureAppLogging`

Change the app logging configuration. By default, redirects all logs
(all namespaces and all levels) to the XUnit output helper. If the test
fails, you get all the tested application logs in the test output.

### `ConfigureAppServices`

This method allows to override dependency injection services of the
tested application. This is useful to inject class doubles. Whenever the
tested application will try to resolve a service declared here, they will
get instances of the `FakeService` instead of the original types:

```cs
// Override services with custom implementations
protected override void ConfigureAppServices(IServiceCollection services)
{
    services.AddScoped<IMyScopedService, FakeService>();
    services.AddSingleton<IMySingletonService, FakeService>();
    services.AddTransient<IMyTransientService, FakeService>();
}
```

You can also overide the lifetime of the service:

```cs
// Override a scoped service with a singleton object
protected override void ConfigureAppServices(IServiceCollection services)
=> services.AddSingleton<IMyScopedService>(new FakeInstance());
```

## Accessing the tested application

### `Client`

You can access the tested application using the `Client` property which
returns a pseudo `HttpClient` created by `WebApplicationFactory`. You
access your application like a client application would:

```cs
[Fact]
public async Task OnTest()
{
    var response = await Client.GetFromJsonAsync<OrderDto>($"/order");
    response.Should().HaveValue();
}
```

### `Configuration`

This property grants you acces to the `IConfiguration` values that are
currently available to the tested application.

### `Services`

This property grants you an access to the DI service container of the
tested application:

```cs
[Fact] public void Test()
{
    var service = Services.GetService<IMyService>();
}
```

Scopes are handled by the framework at the request level, so the test
and the tested application cannot share a common scope. This means that
if you resolve a scoped service from the test, you will get a different
instance than the one used in the tested application.

```cs
[Fact] public void Test()
{
    // these two instances are not the same !
    var expected = Services.GetService<IMyScopedService>().GetHashCode();
    var actual = await Client.GetScopedServiceHashCode();
    actual.Should().Be(expected); // FAIL
}
```

If you need the test class and the tested application to share object
instances, you need to override their lifetime to singleton. Beware
of impacts due to the usage of a singleton object accross multiple
request.

```cs
// Override a scoped service with a singleton instance
protected override void ConfigureAppServices(IServiceCollection services)
    => services.AddSingleton<IMyScopedService, FakeService>();

[Fact] public void Test()
{
    var expected = Services.GetService<IMyScopedService>().GetHashCode();
    var actual = await Client.GetScopedServiceHashCode(); // SUCCESS
}
```

## Extending the behavior of the test class

You can override the following methods to run code before and after each
test:

### `InitializeAsync`

Code executed before the execution of each test of the class.

### `DisposeAsync`

Code executed after the execution of each test of the class.

## EntityFrameworkCore integration

The library provides specific support for efcore. You can achieve this
integration by extending the
`IntegrationTestBase<TEntryPoint, TContext>` class instead of the one
that only uses the entrypoint.

### Configure database driver

You will need to override the abstract `ConfigureDbContext` method to
tell the dependency injection library how to configure your context. A
context instance will be generated per test and injected in your target
app. You can access the same context instance in your test through the
`Database` property.

```cs
protected override void ConfigureDbContext(DbContextOptionsBuilder builder)
    => builder.UseSqlite($"Data Source=test.sqlite");
```

### Database lifetime strategy

The base class also exposes a `DatabaseTestStrategy` property that
allows you to customize the test behavior regarding the database. You
can write your own implementation which requires you to write specific
code that will run before and after each test to set your database.

The library comes with 3 standard behaviors:

#### `IDatabaseTestStrategy<TContext>.Default`

By default the library simply instantates the context and disposes the
instance after the test execution.
The DbContext instance of this strategy is scoped. This means that in
the test code, the `Database` property is not the same instance as the
one in the tested application.

#### `IDatabaseTestStrategy<TContext>.Transaction`

This behavior will execute each test in a separate transaction. This can
be used to prevent write operations to change the contents of the
database. Obviously requires a database engine that supports
transactions.
The DbContext instance of this strategy is singleton. This means that in
the test code, the `Database` property is the same instance as the one
in the tested application.

#### `IDatabaseTestStrategy<TContext>.DatabasePerTest`

This behavior creates a fresh database before test execution and drops
it afterwards. It also applies migrations if any are found, otherwise it
will use `EnsureCreated` (read [Create and Drop
APIs](https://learn.microsoft.com/en-us/ef/core/managing-schemas/ensure-created)
to understand how this might affect your test results). This allows test
parallelization when transaction isolation is not sufficient or
unavailable.
The DbContext instance of this strategy is scoped. This means that in
the test code, the `Database` property is not the same instance as the
one in the tested application.

You MUST combine this behavior with a random name
interpolation in the connection string to run each test on it’s own
database in parallel. Otherwise the tests will try to access the same
database concurrently and will fail to drop it while other tests are
running:

```cs
protected override IDatabaseTestStrategy<Context> DatabaseTestStrategy
    => IDatabaseTestStrategy<Context>.DatabasePerTest;

protected override void ConfigureDbContext(DbContextOptionsBuilder builder)
    => builder.UseSqlite($"Data Source={Guid.NewGuid()}.sqlite");
```

CAUTION: This beahvior WILL drop your database after each test !

### Cleaning the change tracker

The database test strategy determines the lifetime of the DbContext
instance in the DI container.

When running the tests with a singleton DbContext, any call to the
DbContext during the arrange phase (including calls via the HttpClient)
might clogger the Change Tracker with existing entities. In this situation
the DbContext's ChangeTracker might not be empty when the system under test
is called. This may in turn cause attaching entities to fail, whereas it
would have worked in a real request. In such case, the test writer should
call `Database.ChangeTracker.Clear()` at the end of the arrange phase.

Cleaning the change tracker is not needed for scoped or transient.

## OpenTelemetry integration

The library provides specific support for otel. You can achieve this
integration by overriding the `OpenTelemetrySourceNames` property.
All traces that match one of the provided source names will get caught
in-memory and will be accessible through the `Activities` property.

You can use this feature to ensure that a specific branch has been called:

```cs
public class OpenTelemetryTests(ITestOutputHelper output) : IntegrationTestBase<Program>(output)
{
    protected override string[] OpenTelemetrySourceNames => ["SourceA"];

    [Fact]
    public async Task Otel()
    {
        await Client.GetAsync("/api/otel");
        
        // Check for activity existence
        Activities.Any(activity => activity.DisplayName == "return-null").Should().BeTrue();
    }
}
```

You can also use this feature to do performance tests:

```cs
public class OpenTelemetryTests(ITestOutputHelper output) : IntegrationTestBase<Program>(output)
{
    protected override string[] OpenTelemetrySourceNames => ["SourceA"];

    private readonly TimeSpan limit = TimeSpan.FromMilliseconds(300);

    [Fact]
    public async Task Otel()
    {
        await Client.GetAsync("/api/otel");
        Activities
            .Where(activity => activity.DisplayName == "database-insert")
            .Should().AllSatisfy(activity =>
                activity.Duration.Should().BeLessThan(limit));
    }
}
```
