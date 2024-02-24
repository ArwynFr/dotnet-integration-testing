using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArwynFr.IntegrationTesting.Tests.Target;

[ApiController]
public class DummyController : ControllerBase
{
    private readonly DummyDbContext _dbContext;

    public DummyController(DummyDbContext dbContext) => _dbContext = dbContext;

    [HttpGet("/api/entities/{name}")]
    public async Task<Results<Ok<DummyEntity>, NotFound<string>>> ExecuteAsync(string name)
    {
        var value = await _dbContext.Entities.FirstOrDefaultAsync(entity => entity.Name == name);
        if (value == null) { return TypedResults.NotFound("Not found"); }
        return TypedResults.Ok(value);
    }
}