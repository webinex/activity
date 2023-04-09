using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Webinex.Activity
{
    internal class ActivityContext : IActivityContext
    {
        private readonly ActivitySystemValues _systemValues;
        
        public ActivityContext(IActivitySystemValues systemValues = null)
        {
            _systemValues = new ActivitySystemValues
            {
                OperationId = systemValues?.OperationId ?? Guid.NewGuid().ToString(),
                PerformedAt = systemValues?.PerformedAt ?? DateTimeOffset.UtcNow,
                UserId = systemValues?.UserId,
            };
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