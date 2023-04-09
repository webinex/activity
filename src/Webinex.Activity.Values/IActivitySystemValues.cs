using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity
{
    public interface IActivitySystemValues
    {
        [NotNull] string OperationId { get; }
        [MaybeNull] string UserId { get; }
        DateTimeOffset PerformedAt { get; }
        bool Success { get; }
        bool System { get; }
    }
}