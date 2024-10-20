﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting;

public abstract class IntegrationTestBase<TProgram, TContext>(ITestOutputHelper output) : IntegrationTestBase<TProgram>(output)
where TProgram : class
where TContext : DbContext
{
    protected TContext Database => Services.GetRequiredService<TContext>();

    protected virtual IDatabaseTestStrategy<TContext> DatabaseTestStrategy => IDatabaseTestStrategy<TContext>.Default;

    public override async Task DisposeAsync()
    {
        await DatabaseTestStrategy.DisposeAsync(Database);
        await base.DisposeAsync();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await DatabaseTestStrategy.InitializeAsync(Database);
    }

    protected override void ConfigureAppServices(IServiceCollection services)
    {
        base.ConfigureAppServices(services);
        DatabaseTestStrategy.RegisterDbContext(services, ConfigureDbContext);
    }

    protected abstract void ConfigureDbContext(IServiceProvider services, DbContextOptionsBuilder builder);

}
