﻿using ArwynFr.IntegrationTesting.Tests.Target;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests.EntityFrameworkCore;

public class DatabasePerTestStrategyTests(ITestOutputHelper output) : IntegrationTestBase<Program, DummyDbContext>(output)
{
    [Fact]
    public void PerTestStrategy_creates_database()
    {
        Database.Database.GetDbConnection().Should().NotBeNull();
    }

    protected override IDatabaseTestStrategy<DummyDbContext> DatabaseTestStrategy
        => IDatabaseTestStrategy<DummyDbContext>.DatabasePerTest;

    protected override void ConfigureDbContext(IServiceProvider services, DbContextOptionsBuilder builder)
        => builder.UseSqlite($@"Data Source={Guid.NewGuid()}.sqlite");
}
