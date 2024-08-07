using System.Collections.Generic;

namespace Webinex.Activity;

public interface IActivity : IActivityValue
{
    IDictionary<string, object> Meta { get; }
    IEnumerable<IActivity> Children { get; }
    new IMutableActivitySystemValues SystemValues { get; }
    bool Completed { get; }
    void Complete(bool? success = null);
}