using System.Net.Http.Json;

using ArwynFr.IntegrationTesting.Tests.Target;

using Xunit;

namespace ArwynFr.IntegrationTesting.Tests;

public class DummyDriver(DummyDbContext dbContext, HttpClient client) : TestDriverBase<DummyDriver>
{
    private readonly DummyDbContext _dbContext = dbContext;
    private readonly HttpClient _client = client;
    private string? result;

    public async Task ThereIsAnEntity(string name)
    {
        _dbContext.Entities.Add(new DummyEntity { Name = name });
        await _dbContext.SaveChangesAsync();
    }

    public async Task FindEntityByName(string name)
    {
        result = await _client.GetFromJsonAsync<string>($"/api/entities/{name}");
    }

    public Task ResultShouldBe(string name)
    {
        Assert.Equal(name, result);
        return Task.CompletedTask;
    }
}