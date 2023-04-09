using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity
{
    public interface IActivityValue
    {
        [NotNull] string Id { get; }

        [NotNull] string Kind { get; }

        [MaybeNull] string ParentId { get; }

        [NotNull] IActivitySystemValues SystemValues { get; }

        [NotNull] ActivityValues Values { get; }
    }

    public static class ActivityValueExtensions
    {
        public static T Get<T>([NotNull] this IActivityValue value, [NotNull] string path, T defaultValue = default(T))
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