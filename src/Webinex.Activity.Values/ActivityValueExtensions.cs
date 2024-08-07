using System;
using System.Collections;

namespace Webinex.Activity;

public static class ActivityValueExtensions
{
    public static T? Get<T>(this IActivityValue value, string path, T? defaultValue = default)
    {
        value = value ?? throw new ArgumentNullException(nameof(value));
        path = path ?? throw new ArgumentNullException(nameof(path));

        return value.Values.Get(path, defaultValue);
    }

    public static void Enrich(this IActivityValue activityValue, string path, object? val)
    {
        activityValue = activityValue ?? throw new ArgumentNullException(nameof(activityValue));
        path = path ?? throw new ArgumentNullException(nameof(path));

        activityValue.Values.Enrich(path, val);
    }

    public static void Enrich(this IActivityValue activityValue, object value)
    {
        activityValue = activityValue ?? throw new ArgumentNullException(nameof(activityValue));
        value = value ?? throw new ArgumentNullException(nameof(value));
        activityValue.Values.Enrich(value);
    }

    public static void Enrich(this IActivityValue activityValue, IDictionary dictionary)
    {
        activityValue = activityValue ?? throw new ArgumentNullException(nameof(activityValue));
        dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        activityValue.Values.Enrich(dictionary);
    }
}