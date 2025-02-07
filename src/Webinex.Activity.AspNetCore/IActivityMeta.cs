using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity.AspNetCore;

internal interface IActivityMeta
{
    bool IsActivity { get; }
    
    [MemberNotNullWhen(true, nameof(IsActivity))]
    string? Kind { get; }
}