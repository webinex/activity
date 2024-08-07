using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Webinex.Activity;

public interface IActivityScope
{
    bool Initialized { get; }
    IActivity? Current { get; }
    IActivityContext Context { get; }
    ActivityPathItem[] OutboundPath { get; }
    IEnumerable<IActivity> Root { get; }
    IDisposableActivity Push(string kind);
    IActivityScope Add(string kind, Action<IActivity>? action = null);
    Task CompleteAsync(bool success = true);
    IActivityBatchValue ToBatch();
}