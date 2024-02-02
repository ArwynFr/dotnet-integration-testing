using Xunit;

namespace ArwynFr.IntegrationTesting;

public abstract class TestDriverBase<TDriver> where TDriver : TestDriverBase<TDriver>
{
    private readonly List<Func<TDriver, Task>> _givens = new();
    private readonly List<Func<TDriver, Task>> _whens = new();
    private readonly List<Func<TDriver, Task>> _thens = new();

    protected TestDriverBase() => Assert.IsType<TDriver>(this);

    public async Task ExecuteAsync()
    {
        foreach (var func in _givens.Concat(_whens).Concat(_thens))
        {
            await func(this);
        }
    }

    public TestDriverBase<TDriver> Given(Func<TDriver, Task> value)
    {
        _givens.Add(value);
        return this;
    }

    public TestDriverBase<TDriver> When(Func<TDriver, Task> value)
    {
        _whens.Add(value);
        return this;
    }

    public TestDriverBase<TDriver> Then(Func<TDriver, Task> value)
    {
        _thens.Add(value);
        return this;
    }

    public static implicit operator TDriver(TestDriverBase<TDriver> item) => (TDriver)item;
}