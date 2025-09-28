using System.Diagnostics;

namespace DevKit.MediatR.Pipelines;

public static class ApplicationDiagnostics
{
    public const string ActivitySourceName = "DevKit.MediatR";
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public static Activity? StartActivity(string activityName) => ActivitySource.StartActivity(activityName);
}
