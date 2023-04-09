using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity
{
    // ReSharper disable once PossibleInterfaceMemberAmbiguity
    public interface IActivity : IActivityValue
    {
        [NotNull] IDictionary<string, object> Meta { get; }

        [NotNull] IEnumerable<IActivity> Children { get; }
        
        [NotNull] new IMutableActivitySystemValues SystemValues { get; }
        
        bool Completed { get; }

        void Complete(bool? success = null);
    }
}