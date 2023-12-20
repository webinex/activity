using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Webinex.Activity
{
    public interface IActivityScope
    {
        IActivity? Current { get; }
        IActivityContext Context { get; }
        ActivityPathItem[] OutboundPath { get; }
        IEnumerable<IActivity> Root { get; }
        IDisposableActivity Push(string kind);
        Task CompleteAsync(bool success = true);
        IActivityBatchValue ToBatch();
    }
}