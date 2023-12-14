using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity
{
    public interface IMutableActivitySystemValues : IActivitySystemValues
    {
        new string OperationId { get; set; }
        new string? UserId { get; set; }
        new DateTimeOffset PerformedAt { get; set; }
        new bool Success { get; set; }
        new bool System { get; set; }
    }
}