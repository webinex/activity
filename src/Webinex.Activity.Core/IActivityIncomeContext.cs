using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity
{
    public interface IActivityIncomeContext
    {
        [MaybeNull]
        IActivitySystemValues? SystemValues { get; }
        
        [NotNull]
        ActivityPathItem[] Path { get; }
    }
}