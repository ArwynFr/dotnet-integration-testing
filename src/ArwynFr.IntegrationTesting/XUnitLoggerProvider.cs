using System.Linq.Expressions;

using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting;

internal sealed class XUnitLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper output;

    public XUnitLoggerProvider(ITestOutputHelper output) => this.output = output;

    public ILogger CreateLogger(string categoryName) => new XunitLogger(categoryName, output);

    public void Dispose() => Expression.Empty();
}
