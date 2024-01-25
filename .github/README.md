This library provides utility classes for writing integration tests in
dotnet using `XUnit` and `WebApplicationFactory`.

![Nuget.org](https://img.shields.io/nuget/v/ArwynFr.IntegrationTesting?style=for-the-badge)
![Nuget.org](https://img.shields.io/nuget/dt/ArwynFr.IntegrationTesting?style=for-the-badge)
![GitHub
License](https://img.shields.io/github/license/ArwynFr/dotnet-integration-testing?style=for-the-badge)

# Installation

    dotnet add package ArwynFr.IntegrationTesting

# Usage

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

## EntityFrameworkCore integration

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

# Contributing

This project welcomes contributions:

<table>
<colgroup>
<col style="width: 15%" />
<col style="width: 85%" />
</colgroup>
<tbody>
<tr class="odd">
<td><p>Request for support</p></td>
<td><p>TBD</p></td>
</tr>
<tr class="even">
<td><p>Report malfunctions</p></td>
<td><p>Please <a
href="https://github.com/ArwynFr/dotnet-integration-testing/issues/new/choose">create
a new issue on GitHub</a></p></td>
</tr>
<tr class="odd">
<td><p>Suggest a feature</p></td>
<td><p>Please <a
href="https://github.com/ArwynFr/dotnet-integration-testing/issues/new/choose">create
a new issue on GitHub</a></p></td>
</tr>
<tr class="even">
<td><p>Offer some code</p></td>
<td><p>Please <a
href="https://github.com/ArwynFr/dotnet-integration-testing/fork">fork
the repository</a> and <a
href="https://github.com/ArwynFr/dotnet-integration-testing/compare">submit
a pull-request</a><br />
Read our definition of done in <a
href="https://github.com/ArwynFr/dotnet-integration-testing/blob/main/.github/CONTRIBUTING.adoc">contributing
guidelines</a></p></td>
</tr>
<tr class="odd">
<td><p>Disclose vulnerability</p></td>
<td><p>Please <a
href="https://github.com/ArwynFr/dotnet-integration-testing/security/advisories">create
a new security advisory on GitHub</a></p></td>
</tr>
</tbody>
</table>
