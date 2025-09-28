using System.Diagnostics;

namespace DevKit.Base;

public static class ActivityExtensions
{
    public static void FinishActivity(this Activity? activity, ActivityEvent? activityEvent = null)
    {
        if (activityEvent.HasValue)
        {
            activity?.AddEvent(activityEvent.Value);
        }

        activity?.Dispose();
    }

    public static void SetFailure(this Activity? activity, Exception? exception = null, IEnumerable<KeyValuePair<string, object?>>? tags = null)
    {
        if (exception is not null)
        {
            tags ??= [];
            activity?.AddException(exception, new TagList([..tags]));
        }

        activity?.SetStatus(ActivityStatusCode.Error);
    }
}
