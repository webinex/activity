using System;

namespace Webinex.Activity.Server.DataAccess;

public class ActivityRowBase
{
    public int Id { get; protected set; }
    public string Uid { get; protected set; } = null!;
    public string Kind { get; protected set; } = null!;
    public string OperationUid { get; protected set; } = null!;
    public string? TenantId { get; protected set; }
    public string? UserId { get; protected set; }
    public bool Success { get; protected set; }
    public DateTimeOffset PerformedAt { get; protected set; }
    public bool System { get; protected set; }
    public string? ParentUid { get; protected set; }

    protected ActivityRowBase()
    {
    }
    
    protected ActivityRowBase(IActivityValue value)
    {
        value = value ?? throw new ArgumentNullException(nameof(value));
        Uid = value.Id;
        Kind = value.Kind;
        OperationUid = value.SystemValues.OperationId;
        Success = value.SystemValues.Success;
        UserId = value.SystemValues.UserId;
        TenantId = value.SystemValues.TenantId;
        PerformedAt = value.SystemValues.PerformedAt;
        System = value.SystemValues.System;
    }
}