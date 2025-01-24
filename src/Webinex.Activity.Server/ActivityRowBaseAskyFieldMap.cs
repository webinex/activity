using System;
using System.Linq.Expressions;
using Webinex.Activity.Server.DataAccess;
using Webinex.Asky;

namespace Webinex.Activity.Server;

public class ActivityRowBaseAskyFieldMap<TActivityRow> : IAskyFieldMap<TActivityRow>
    where TActivityRow : ActivityRowBase
{
    public virtual Expression<Func<TActivityRow, object>>? this[string fieldId]
    {
        get
        {
            fieldId = fieldId ?? throw new ArgumentNullException(nameof(fieldId));
                
            return fieldId switch
            {
                "_id" => x => x.Id,
                "id" => x => x.Uid,
                "kind" => x => x.Kind,
                "operationId" => x => x.OperationUid,
                "tenantId" => x => x.TenantId!,
                "userId" => x => x.UserId!,
                "success" => x => x.Success,
                "performedAt" => x => x.PerformedAt,
                "system" => x => x.System,
                "parentId" => x => x.ParentUid!,
                _ => null,
            };
        }
    }
}