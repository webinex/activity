using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Webinex.Activity
{
    internal class Activity : IDisposableActivity
    {
        private readonly LinkedList<Activity> _children = new LinkedList<Activity>();
        private readonly ILogger _logger;

        public Activity(
            string kind,
            string? parentId,
            IActivitySystemValues systemValues,
            ILogger<Activity> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Id = Guid.NewGuid().ToString();
            ParentId = parentId;
            Kind = kind ?? throw new ArgumentNullException(nameof(kind));
            SystemValues = new ActivitySystemValues(systemValues);
        }

        public string Kind { get; }

        public string Id { get; }

        public string? ParentId { get; }

        public IMutableActivitySystemValues SystemValues { get; set; }

        IActivitySystemValues IActivityValue.SystemValues => SystemValues;

        public IDictionary<string, object> Meta { get; private set; } = new Dictionary<string, object>();

        public ActivityValues Values { get; private set; } = new ActivityValues();

        public IEnumerable<IActivity> Children => _children;

        public void PushChild(Activity activity)
        {
            activity = activity ?? throw new ArgumentNullException(nameof(activity));
            _children.AddLast(activity);
        }

        public bool Completed { get; private set; }

        public void Complete(bool? success = null)
        {
            if (Completed)
            {
                _logger.ActivityAlreadyCompleted(this);
                return;
            }

            if (success.HasValue)
            {
                SystemValues.Success = success.Value;
            }

            if (NotCompletedChildren.Any())
                _logger.NotCompletedChildren(NotCompletedChildren);

            foreach (var activity in NotCompletedChildren)
                activity.Complete();

            Meta = Meta.ToImmutableDictionary();
            Values.Freeze();
            Completed = true;
        }

        private Activity[] NotCompletedChildren => _children.Where(x => !x.Completed).ToArray();

        public void Dispose()
        {
            Complete();
        }
    }
}