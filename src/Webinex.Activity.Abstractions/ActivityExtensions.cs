using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Webinex.Activity;

public static class ActivitiesExtensions
{
    public static IActivity[] All(this IActivityScope activityScope)
    {
        activityScope = activityScope ?? throw new ArgumentNullException(nameof(activityScope));
        return activityScope.Root.SelectMany(x => WithFlattenChildren(x)).ToArray();
    }

    public static ActivityPathItem[] Path(this IActivityScope activityScope)
    {
        activityScope = activityScope ?? throw new ArgumentNullException(nameof(activityScope));
        var current = activityScope.Current;
        if (current == null) return Array.Empty<ActivityPathItem>();

        var path = new LinkedList<ActivityPathItem>();
        var all = activityScope.All().ToDictionary(x => x.Id);
        var pointer = current;

        while (pointer != null)
        {
            path.AddLast(new ActivityPathItem(pointer.Id, pointer.Kind));

            if (pointer.ParentId == null || !all.ContainsKey(pointer.ParentId))
            {
                pointer = null;
                continue;
            }

            pointer = all[pointer.ParentId];
        }

        var result = path.Reverse().ToArray();
        return activityScope.OutboundPath.Concat(result).ToArray();
    }

    public static IActivity? FindLastNotCompleted(this IActivityScope activityScope)
    {
        return FindLastNotCompleted(activityScope.Root);
    }

    private static IActivity? FindLastNotCompleted(IEnumerable<IActivity> activities)
    {
        var notCompleted = activities.LastOrDefault(x => !x.Completed);

        return notCompleted == null
            ? null
            : FindLastNotCompleted(notCompleted.Children) ?? notCompleted;
    }

    public static IEnumerable<IActivity> WithFlattenChildren(this IActivity activity)
    {
        activity = activity ?? throw new ArgumentNullException(nameof(activity));

        if (!activity.Children.Any())
            return new[] { activity };

        return activity.Children.SelectMany(WithFlattenChildren).Append(activity);
    }

    public static IActivity Enrich(this IActivity activity, string path, object? val)
    {
        activity = activity ?? throw new ArgumentNullException(nameof(activity));
        path = path ?? throw new ArgumentNullException(nameof(path));

        activity.Values.Enrich(path, val);
        return activity;
    }

    public static IActivity Enrich(this IActivity activity, object value)
    {
        activity = activity ?? throw new ArgumentNullException(nameof(activity));
        value = value ?? throw new ArgumentNullException(nameof(value));
        activity.Values.Enrich(value);
        return activity;
    }

    public static IActivity Enrich(this IActivity activity, IDictionary dictionary)
    {
        activity = activity ?? throw new ArgumentNullException(nameof(activity));
        dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        activity.Values.Enrich(dictionary);
        return activity;
    }
}