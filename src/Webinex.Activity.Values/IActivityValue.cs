using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity
{
    public interface IActivityValue
    {
        string Id { get; }
        string Kind { get; }
        string? ParentId { get; }
        IActivitySystemValues SystemValues { get; }
        ActivityValues Values { get; }
    }

    public static class ActivityValueExtensions
    {
        public static T? Get<T>([NotNull] this IActivityValue value, [NotNull] string path, T? defaultValue = default(T))
        {
            value = value ?? throw new ArgumentNullException(nameof(value));
            path = path ?? throw new ArgumentNullException(nameof(path));

            return value.Values.Get(path, defaultValue);
        }

        public static void Enrich([NotNull] this IActivityValue value, [NotNull] string path, object val)
        {
            value = value ?? throw new ArgumentNullException(nameof(value));
            path = path ?? throw new ArgumentNullException(nameof(path));

            value.Values.Enrich(path, val);
        }
    }
}