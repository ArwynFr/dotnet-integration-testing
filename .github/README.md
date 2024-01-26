# ArwynFr.IntegrationTesting

This library provides utility classes for writing integration tests in
dotnet using `XUnit` and `WebApplicationFactory`.

![Nuget.org](https://img.shields.io/nuget/v/ArwynFr.IntegrationTesting?style=for-the-badge)
![Nuget.org](https://img.shields.io/nuget/dt/ArwynFr.IntegrationTesting?style=for-the-badge)
![GitHub
License](https://img.shields.io/github/license/ArwynFr/dotnet-integration-testing?style=for-the-badge)

## Installation

    dotnet add package ArwynFr.IntegrationTesting

## Usage

Read [advanced usage
documentation](https://github.com/ArwynFr/dotnet-integration-testing/blob/main/.github/USAGE.adoc)
for further details.

By default, the lib redirects the tested application logs to XUnit
output, so you get them in the test output in case of failure. It also
overwrites the application configuration with values from user secrets
and environement variables.

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

### EntityFrameworkCore integration

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

## Contributing

This project welcomes contributions:

**Request for support:**  
TBD

**Disclose vulnerability:**  
Please [create a new security advisory on GitHub](https://github.com/ArwynFr/dotnet-integration-testing/security/advisories)

**Report malfunctions:**  
[Please create a new issue on GitHub](https://github.com/ArwynFr/dotnet-integration-testing/issues/new/choose)

**Suggest a feature:**  
[Please create a new issue on GitHub](https://github.com/ArwynFr/dotnet-integration-testing/issues/new/choose)

**Offer some code:**  
Please [fork the repository](https://github.com/ArwynFr/dotnet-integration-testing/fork)
and [submit a pull-request](https://github.com/ArwynFr/dotnet-integration-testing/compare)
\
[Read our definition of done in contributing guidelines](https://github.com/ArwynFr/dotnet-integration-testing/blob/main/.github/CONTRIBUTING.md)