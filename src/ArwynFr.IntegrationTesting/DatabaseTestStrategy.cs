using Microsoft.EntityFrameworkCore;

namespace ArwynFr.IntegrationTesting;

internal sealed class DatabaseTestStrategy<TContext> : IDatabaseTestStrategy<TContext>
where TContext : DbContext
{
    private bool transaction = false;
    private bool transient = false;
    private bool update = false;

    public async Task DisposeAsync(TContext database)
    {
        if (transient) { await database.Database.EnsureDeletedAsync(); }
    }

    public async Task InitializeAsync(TContext database)
    {
        if (transient) { await database.Database.EnsureDeletedAsync(); }
        if (update || transient) { await UpdateDatabase(database); }
        if (transaction) { await database.Database.BeginTransactionAsync(); }
    }

    public DatabaseTestStrategy<TContext> WithPerTest()
    {
        transient = true;
        return this;
    }

    public DatabaseTestStrategy<TContext> WithSchemaUpdate()
    {
        update = true;
        return this;
    }

    public DatabaseTestStrategy<TContext> WithTransaction()
    {
        transaction = true;
        return this;
    }

    private static Task UpdateDatabase(TContext context)
        => context.Database.GetMigrations().Any()
        ? context.Database.MigrateAsync()
        : context.Database.EnsureCreatedAsync();
}