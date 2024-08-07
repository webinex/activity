using System.Collections.Generic;

namespace Webinex.Activity;

public interface IActivityContext : IActivityContextValue
{
    new IMutableActivitySystemValues SystemValues { get; }
    IDictionary<string, object> Meta { get; }
}