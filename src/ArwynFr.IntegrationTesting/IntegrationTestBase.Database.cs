using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting;

public abstract class IntegrationTestBase<TProgram, TContext>(ITestOutputHelper output) : IntegrationTestBase<TProgram>(output)
where TProgram : class
where TContext : DbContext
{
    protected TContext Database => Services.GetRequiredService<TContext>();

    protected virtual IDatabaseTestStrategy<TContext> DatabaseTestStrategy => IDatabaseTestStrategy<TContext>.Default;

    protected virtual ServiceLifetime DatabaseLifetime => ServiceLifetime.Scoped;

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

        if (!DatabaseTestStrategy.IsLifetimeSupported(DatabaseLifetime))
        {
            throw new InvalidOperationException("Service lifetime not supported by the database strategy");
        }

        services.RemoveAll<TContext>();
        services.RemoveAll<DbContextOptions<TContext>>();
        services.AddDbContext<TContext>(ConfigureDbContext, DatabaseLifetime);
        services.Add(new ServiceDescriptor(typeof(TContext), typeof(TContext), DatabaseLifetime));
    }

    protected abstract void ConfigureDbContext(DbContextOptionsBuilder builder);

}
