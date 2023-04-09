using System;
using System.Linq;
using System.Linq.Expressions;
using Webinex.Activity.Server.EfCore;
using Webinex.Asky;

namespace Webinex.Activity.Server.Controllers
{
    public class ActivityRowAskyFieldMap : IAskyFieldMap<ActivityRow>
    {
        internal const string VALUE_FIELD_ID = "$value";
        internal const string VALUE_FIELD_ID_PREFIX = "$value.";
        internal const string VALUE_VALUE_FIELD_ID = "$value.value";
        internal const string VALUE_SEARCH_PATH_FIELD_ID = "$value.searchPath";

        public Expression<Func<ActivityRow, object>> this[string fieldId]
        {
            get
            {
                fieldId = fieldId ?? throw new ArgumentNullException(nameof(fieldId));
                
                Expression<Func<ActivityRow, object>> expression = fieldId switch
                {
                    "id" => x => x.Uid,
                    "operationId" => x => x.OperationUid,
                    "kind" => x => x.Kind,
                    "parentId" => x => x.ParentUid,
                    "userId" => x => x.UserId,
                    "success" => x => x.Success,
                    "performedAt" => x => x.PerformedAt,
                    VALUE_FIELD_ID => x => x.Values,
                    VALUE_SEARCH_PATH_FIELD_ID => x => x.Values.Select(v => v.SearchPath),
                    VALUE_VALUE_FIELD_ID => x => x.Values.Select(v => v.Value),
                    _ => null,
                };

                return expression == null && fieldId.StartsWith(VALUE_FIELD_ID_PREFIX)
                    ? x => x.Values.First().Value
                    : expression;
            }
        }
    }
}