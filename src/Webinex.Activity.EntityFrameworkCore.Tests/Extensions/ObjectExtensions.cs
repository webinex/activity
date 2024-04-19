namespace Webinex.Activity.EntityFrameworkCore.Tests.Extensions;

public static class ObjectExtensions
{
    public static T As<T>(this object? value)
    {
        return (T)value!;
    }

    public static IDictionary<string, object?> AsValues(this object? value)
    {
        return value.As<IDictionary<string, object?>>();
    }

    public static IDictionary<string, object?>[] AsValuesArray(this object? value)
    {
        return value.As<IDictionary<string, object?>[]>();
    }

    public static IDictionary<string, object?> CloneValues(this IDictionary<string, object?> values)
    {
        return new Dictionary<string, object?>(values.Select(x =>
            new KeyValuePair<string, object?>(x.Key, x.Value?.CloneValue())));
    }

    private static object CloneValue(this object value)
    {
        return value switch
        {
            IDictionary<string, object?> dictionary => dictionary.CloneValues(),
            IDictionary<string, object?>[] array => array.Select(CloneValues).ToArray(),
            _ => value
        };
    }
}