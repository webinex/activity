using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Webinex.Activity
{
    internal static class ActivityLoggerExtensions
    {
        public static void NotCompletedChildren(this ILogger logger, IEnumerable<Activity> notCompleted)
        {
            if (!logger.IsEnabled(LogLevel.Warning))
                return;

            var notCompletedList = string.Join(", ", notCompleted.Select(x => x.Id.ToString()));
            logger.LogWarning(
                "Completing scope or action with not completed children: {List}. It would be automatically completed, but it might be explicitly completed",
                notCompletedList);
        }

        public static void ActivityAlreadyCompleted(this ILogger logger, Activity activity)
        {
            if (!logger.IsEnabled(LogLevel.Warning))
                return;

            logger.LogWarning("Activity {Id} already completed", activity.Id);
        }
    }
}