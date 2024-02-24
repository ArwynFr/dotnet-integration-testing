using System.Diagnostics;

public static class ActivityHelper
{
  public const string ActivityName = "otel-test";
  private const string SourceName = "Dummy";
  public static ActivitySource Source { get; } = new ActivitySource(SourceName);
}