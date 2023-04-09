using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity
{
    public class ActivityPathItem
    {
        public ActivityPathItem([NotNull] string id, [NotNull] string kind)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Kind = kind ?? throw new ArgumentNullException(nameof(kind));
        }

        [NotNull]
        public string Id { get; }

        [NotNull]
        public string Kind { get; }
    }
}