using ArwynFr.IntegrationTesting.Tests.Target;

using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IDummyService, DummyService>();
builder.Services.AddSqlite<DummyDbContext>("invalid");
builder.Services.AddControllers();
var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapGet("/error", () => { throw new DummyException(); });
app.MapGet("/service", (IDummyService service) => service.GetHashCode());
app.MapGet("/otel", async () => {
  using (var activity = ActivityHelper.Source.StartActivity(ActivityHelper.ActivityName))
  {
    await Task.Delay(TimeSpan.FromMilliseconds(10));
  }
});
app.MapControllers();
app.Run();

public partial class Program;