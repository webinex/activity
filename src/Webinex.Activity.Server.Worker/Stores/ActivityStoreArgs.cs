using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity.Server.Worker.Stores
{
    public class ActivityStoreArgs
    {
        public ActivityStoreArgs([NotNull] IActivityValue value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull]
        public IActivityValue Value { get; }
    }
}