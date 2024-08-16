using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArwynFr.IntegrationTesting;

internal sealed class DatabaseTestStrategy<TContext> : IDatabaseTestStrategy<TContext>
where TContext : DbContext
{
    private bool transaction = false;
    private bool transient = false;

    public async Task DisposeAsync(TContext database)
    {
        if (transient)
        {
            await database.Database.EnsureDeletedAsync();
        }
    }

    public async Task InitializeAsync(TContext database)
    {
        if (transient)
        {
            await database.Database.EnsureDeletedAsync();
            await UpdateDatabase(database);
        }
        if (transaction)
        {
            await database.Database.BeginTransactionAsync();
        }
    }

    public DatabaseTestStrategy<TContext> WithPerTest()
    {
        transient = true;
        return this;
    }

    public DatabaseTestStrategy<TContext> WithTransaction()
    {
        transaction = true;
        return this;
    }

    private static Task UpdateDatabase(TContext context)
        => context.Database.IsRelational()
        && context.Database.GetMigrations().Any()
        ? context.Database.MigrateAsync()
        : context.Database.EnsureCreatedAsync();

    public bool IsLifetimeSupported(ServiceLifetime lifetime)
    => transaction ? lifetime == ServiceLifetime.Singleton : true;
}
