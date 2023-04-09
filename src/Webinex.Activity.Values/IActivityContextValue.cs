using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity
{
    public interface IActivityContextValue
    {
        [NotNull]
        IActivitySystemValues SystemValues { get; }
    }
}