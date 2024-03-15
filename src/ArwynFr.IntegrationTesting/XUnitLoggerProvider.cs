using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting;

internal sealed class XUnitLoggerProvider(ITestOutputHelper output) : ILoggerProvider
{
    private readonly ITestOutputHelper output = output;

    public ILogger CreateLogger(string categoryName) => new XunitLogger(categoryName, output);

    public void Dispose() => Expression.Empty();
}
