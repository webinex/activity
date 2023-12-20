using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Webinex.Activity
{
    internal class ActivityContext : IActivityContext
    {
        private readonly ActivitySystemValues _systemValues;

        public ActivityContext(IActivitySystemValues? systemValues = null)
        {
            _systemValues = new ActivitySystemValues(
                operationId: systemValues?.OperationId ?? Guid.NewGuid().ToString(),
                performedAt: systemValues?.PerformedAt ?? DateTimeOffset.UtcNow,
                userId: systemValues?.UserId,
                tenantId: systemValues?.TenantId);
        }

        public IMutableActivitySystemValues SystemValues => _systemValues;
        IActivitySystemValues IActivityContextValue.SystemValues => _systemValues;
        public IDictionary<string, object> Meta { get; private set; } = new Dictionary<string, object>();


        public void Freeze()
        {
            _systemValues.Freeze();
            Meta = Meta.ToImmutableDictionary();
        }
    }
}