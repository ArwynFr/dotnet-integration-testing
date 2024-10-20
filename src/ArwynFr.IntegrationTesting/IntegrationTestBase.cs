﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting;

public abstract class IntegrationTestBase<TProgram> : IAsyncLifetime
where TProgram : class
{
    private readonly XUnitLoggerProvider provider;

    private readonly WebApplicationFactory<TProgram> factory;

    protected IntegrationTestBase(ITestOutputHelper output)
    {
        provider = new XUnitLoggerProvider(output);
        factory = new WebApplicationFactory<TProgram>().WithWebHostBuilder(ConfigureWebHostBuilder);
        Client = factory.CreateClient();
        Services = factory.Services.CreateScope().ServiceProvider;
        Configuration = factory.Services.GetRequiredService<IConfiguration>();
    }

    protected IEnumerable<Activity> Activities { get; } = new List<Activity>();

    protected HttpClient Client { get; }

    protected IConfiguration Configuration { get; }

    protected IServiceProvider Services { get; }

    protected virtual string[] OpenTelemetrySourceNames { get; } = [];

    public virtual Task DisposeAsync()
    {
        provider.Dispose();
        factory.Dispose();
        return Task.CompletedTask;
    }

    public virtual Task InitializeAsync() => Task.CompletedTask;

    protected virtual void ConfigureAppConfiguration(WebHostBuilderContext context, IConfigurationBuilder builder) => builder
        .AddJsonFile("appsettings.json", optional: true)
        .AddUserSecrets(typeof(TProgram).Assembly)
        .AddUserSecrets(GetType().Assembly)
        .AddEnvironmentVariables();

    protected virtual void ConfigureAppLogging(WebHostBuilderContext context, ILoggingBuilder builder)
    {
        builder.ClearProviders();
        builder.AddProvider(provider);
    }

    protected virtual void ConfigureAppServices(IServiceCollection services) => services.AddOpenTelemetry()
        .WithTracing(builder => builder
            .AddSource(OpenTelemetrySourceNames)
            .AddAspNetCoreInstrumentation()
            .AddInMemoryExporter(Activities as ICollection<Activity>));

    protected virtual void ConfigureWebHostBuilder(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(ConfigureAppConfiguration);
        builder.ConfigureLogging(ConfigureAppLogging);
        builder.ConfigureTestServices(ConfigureAppServices);
    }
}
