using System;

namespace Webinex.Activity
{
    public interface IActivitySystemValues
    {
        string OperationId { get; }
        string? UserId { get; }
        DateTimeOffset PerformedAt { get; }
        bool Success { get; }
        bool System { get; }
    }
}