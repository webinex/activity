using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity
{
    public interface IActivityBatchValue
    {
        [NotNull]
        IActivityContextValue Context { get; }
        
        [NotNull]
        IEnumerable<IActivityValue> Activities { get; }
    }
}