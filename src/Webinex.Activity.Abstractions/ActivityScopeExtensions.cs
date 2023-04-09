using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity
{
    public static class ActivityScopeExtensions
    {
        public static IActivityScope Enrich(
            [NotNull] this IActivityScope activityScope,
            [NotNull] string path,
            object value)
        {
            activityScope = activityScope ?? throw new ArgumentNullException(nameof(activityScope));
            path = path ?? throw new ArgumentNullException(nameof(path));

            if (activityScope.Current == null)
                throw new InvalidOperationException("No current action");

            activityScope.Current.Enrich(path, value);
            return activityScope;
        }
    }
}