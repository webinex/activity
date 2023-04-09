using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Webinex.Activity
{
    public interface IActivityScope
    {
        [MaybeNull] IActivity Current { get; }
        [NotNull] IActivityContext Context { get; }
        [NotNull] ActivityPathItem[] OutboundPath { get; }
        [NotNull] IEnumerable<IActivity> Root { get; }
        IDisposableActivity Push([NotNull] string kind);
        Task CompleteAsync(bool success = true);
        IActivityBatchValue ToBatch();
    }
}