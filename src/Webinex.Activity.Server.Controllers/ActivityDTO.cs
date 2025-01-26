using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Webinex.Activity.Server.Controllers;

public class ActivityDTO
{
    public string Id { get; }
    public string Kind { get; }
    public string OperationId { get; }
    public string? TenantId { get; }
    public string? UserId { get; }
    public bool Success { get; }
    public DateTimeOffset PerformedAt { get; }
    public string? ParentId { get; }

    [JsonExtensionData]
    public IDictionary<string, object?>? Extra { get; }

    public ActivityDTO(
        string id,
        string kind,
        string operationId,
        string? tenantId,
        string? userId,
        bool success,
        DateTimeOffset performedAt,
        string? parentId,
        IDictionary<string, object?>? extension)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Kind = kind ?? throw new ArgumentNullException(nameof(kind));
        OperationId = operationId ?? throw new ArgumentNullException(nameof(operationId));
        TenantId = tenantId;
        UserId = userId;
        Success = success;
        PerformedAt = performedAt;
        ParentId = parentId;
        Extra = extension;
    }
}