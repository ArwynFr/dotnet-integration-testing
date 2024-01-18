using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting;

public abstract class IntegrationTestBase<TProgram> : IAsyncLifetime
where TProgram : class
{
    private readonly ITestOutputHelper output;

    protected IntegrationTestBase(ITestOutputHelper output)
    {
        this.output = output;
        var factory = new WebApplicationFactory<TProgram>().WithWebHostBuilder(ConfigureWebHostBuilder);
        Client = factory.CreateClient();
        Services = factory.Services;
        Configuration = factory.Services.GetRequiredService<IConfiguration>();
    }

    protected HttpClient Client { get; }

    protected IConfiguration Configuration { get; }

    protected IServiceProvider Services { get; }

    public virtual Task DisposeAsync() => Task.CompletedTask;

    public virtual Task InitializeAsync() => Task.CompletedTask;

    protected virtual void ConfigureAppConfiguration(WebHostBuilderContext context, IConfigurationBuilder builder) => builder
        .AddJsonFile("appsettings.json", optional: true)
        .AddUserSecrets(typeof(TProgram).Assembly)
        .AddUserSecrets(GetType().Assembly)
        .AddEnvironmentVariables();

    protected virtual void ConfigureAppLogging(WebHostBuilderContext context, ILoggingBuilder builder)
    {
        builder.ClearProviders();
        builder.AddProvider(new XUnitLoggerProvider(output));
    }

    protected virtual void ConfigureAppServices(IServiceCollection services) { }

    protected virtual void ConfigureWebHostBuilder(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(ConfigureAppConfiguration);
        builder.ConfigureLogging(ConfigureAppLogging);
        builder.ConfigureTestServices(ConfigureAppServices);
    }
}