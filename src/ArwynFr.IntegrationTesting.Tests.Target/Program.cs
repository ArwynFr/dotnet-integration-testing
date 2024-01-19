using ArwynFr.IntegrationTesting.Tests.Target;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IDummyService, DummyService>();
builder.Services.AddSqlite<DummyDbContext>("invalid");
var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapGet("/service", (IDummyService service) => service.GetHashCode());
app.Run();

public partial class Program;