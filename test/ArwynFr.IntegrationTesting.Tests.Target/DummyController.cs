using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArwynFr.IntegrationTesting.Tests.Target;

[ApiController, Route("/api/entities")]
public class DummyController(DummyDbContext dbContext) : ControllerBase
{
    private readonly DummyDbContext _dbContext = dbContext;

    [HttpGet("{name}")]
    public async Task<Results<Ok<DummyEntity>, NotFound<string>>> ExecuteAsync(string name)
    {
        var value = await _dbContext.Entities.FirstOrDefaultAsync(entity => entity.Name == name);
        return value == null ? (Results<Ok<DummyEntity>, NotFound<string>>)TypedResults.NotFound("Not found") : (Results<Ok<DummyEntity>, NotFound<string>>)TypedResults.Ok(value);
    }
}
