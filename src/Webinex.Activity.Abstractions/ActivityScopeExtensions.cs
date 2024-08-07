using System;
using System.Collections;

namespace Webinex.Activity;

public static class ActivityScopeExtensions
{
    public static IActivityScope Enrich(
        this IActivityScope activityScope,
        string path,
        object? value)
    {
        activityScope = activityScope ?? throw new ArgumentNullException(nameof(activityScope));
        path = path ?? throw new ArgumentNullException(nameof(path));

        if (!activityScope.Initialized)
            throw new InvalidOperationException("Activity scope not initialized");

        activityScope.Current!.Enrich(path, value);
        return activityScope;
    }

    public static IActivityScope Enrich(this IActivityScope activityScope, object value)
    {
        activityScope = activityScope ?? throw new ArgumentNullException(nameof(activityScope));

        if (!activityScope.Initialized)
            throw new InvalidOperationException("Activity scope not initialized");

        activityScope.Current!.Enrich(value);
        return activityScope;
    }

    public static IActivityScope Enrich(this IActivityScope activityScope, IDictionary dictionary)
    {
        activityScope = activityScope ?? throw new ArgumentNullException(nameof(activityScope));

        if (!activityScope.Initialized)
            throw new InvalidOperationException("Activity scope not initialized");

        activityScope.Current!.Enrich(dictionary);
        return activityScope;
    }

    public static IActivityScope Add(this IActivityScope activityScope, string kind, object value)
    {
        activityScope = activityScope ?? throw new ArgumentNullException(nameof(activityScope));
        kind = kind ?? throw new ArgumentNullException(nameof(kind));
        value = value ?? throw new ArgumentNullException(nameof(value));

        if (!activityScope.Initialized)
            throw new InvalidOperationException("Activity scope not initialized");
        
        return activityScope.Add(kind, activity => activity.Enrich(value));
    }

    public static IActivityScope Add(this IActivityScope activityScope, string kind, IDictionary value)
    {
        activityScope = activityScope ?? throw new ArgumentNullException(nameof(activityScope));
        kind = kind ?? throw new ArgumentNullException(nameof(kind));
        value = value ?? throw new ArgumentNullException(nameof(value));

        if (!activityScope.Initialized)
            throw new InvalidOperationException("Activity scope not initialized");
        
        activityScope.Add(kind, activity => activity.Enrich(value));
        return activityScope;
    }
}