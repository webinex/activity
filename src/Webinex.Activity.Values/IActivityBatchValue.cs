using System.Collections.Generic;

namespace Webinex.Activity
{
    public interface IActivityBatchValue
    {
        IActivityContextValue Context { get; }
        IEnumerable<IActivityValue> Activities { get; }
    }
}