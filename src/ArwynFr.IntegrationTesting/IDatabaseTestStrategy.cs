using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArwynFr.IntegrationTesting;

public interface IDatabaseTestStrategy<TContext>
where TContext : DbContext
{
    /// <summary>
    /// This strategy will create the database on test start and delete the database after execution.
    /// You should generate datbase name randomly using Guid.NewGuid() to prevent errors.
    /// </summary>
    public static IDatabaseTestStrategy<TContext> DatabasePerTest => new DatabaseTestStrategy<TContext>().WithPerTest();

    /// <summary>
    /// The default strategy is to do nothing
    /// </summary>
    public static IDatabaseTestStrategy<TContext> Default => new DatabaseTestStrategy<TContext>();

    /// <summary>
    /// This strategy will use a transaction for each test
    /// </summary>
    public static IDatabaseTestStrategy<TContext> Transaction => new DatabaseTestStrategy<TContext>().WithTransaction();

    Task DisposeAsync(TContext database);
    Task InitializeAsync(TContext database);
    void RegisterDbContext(IServiceCollection services, Action<IServiceProvider, DbContextOptionsBuilder> configureDbContext);
}
