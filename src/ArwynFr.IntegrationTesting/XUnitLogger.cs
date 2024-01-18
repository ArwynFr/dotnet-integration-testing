using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting;

internal sealed class XunitLogger : ILogger
{
    private readonly string categoryName;
    private readonly ITestOutputHelper output;

    public XunitLogger(string categoryName, ITestOutputHelper output)
    {
        this.categoryName = categoryName;
        this.output = output;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var level = logLevel.ToString().AsSpan(0, 5).ToString().ToUpper();
        output.WriteLine($"{level}:\t{categoryName}{Environment.NewLine}\t{formatter(state, exception)}");
        if (exception != null)
        {
            output.WriteLine(string.Empty);
            output.WriteLine(exception.ToString());
        }
        output.WriteLine(string.Empty);
    }
}