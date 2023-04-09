using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Webinex.Activity
{
    internal class ActivityScope : IActivityScope, IActivityBatchValue
    {
        private readonly LinkedList<Activity> _root = new();
        private readonly IActivityStore _store;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly IActivityIncomeContext _incomeContext;

        private readonly Lazy<ActivityContext> _contextLazy;
        private bool _completed = false;

        public ActivityScope(
            IActivityStore store,
            ILoggerFactory loggerFactory,
            IEnumerable<IActivityContextInitializer> contextInitializers,
            IActivityIncomeContext incomeContext)
        {
            _store = store;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<ActivityScope>();
            _incomeContext = incomeContext;

            _contextLazy = new Lazy<ActivityContext>(() =>
            {
                var context = new ActivityContext(incomeContext.SystemValues);


                foreach (var initializer in contextInitializers)
                    initializer.Initialize(context);

                return context;
            });
        }

        public IActivity Current => this.FindLastNotCompleted();

        public IActivityContext Context => _contextLazy.Value;

        public ActivityPathItem[] OutboundPath => _incomeContext.Path;

        public IEnumerable<IActivity> Root => _root;

        public IDisposableActivity Push(string kind)
        {
            kind = kind ?? throw new ArgumentNullException(nameof(kind));

            var activity = NewActivity(kind);

            if (Current == null)
                _root.AddLast(activity);

            return activity;
        }

        public async Task CompleteAsync(bool success = true)
        {
            AssertScopeNotCompleted();
            _completed = true;
            
            if (!Root.Any())
                return;
            
            if (NotCompletedActivities.Any())
                _logger.NotCompletedChildren(NotCompletedActivities);

            foreach (var activity in NotCompletedActivities)
                activity.Complete(success);

            _contextLazy.Value.SystemValues.Success = success;
            _contextLazy.Value.Freeze();

            await _store.StoreAsync(this);
        }

        private void AssertScopeNotCompleted()
        {
            if (_completed)
            {
                throw new InvalidOperationException("Scope already completed");
            }
        }

        private Activity NewActivity(string kind)
        {
            var logger = _loggerFactory.CreateLogger<Activity>();
            var parent = (Activity)Current;
            var outboundParent = OutboundPath.LastOrDefault();
            var parentId = parent?.Id ?? outboundParent?.Id;
            var systemValues = parent?.SystemValues ?? Context.SystemValues;
            var activity = new Activity(kind, parentId, systemValues, logger);
            parent?.PushChild(activity);
            return activity;
        }

        private Activity[] NotCompletedActivities => _root.Where(x => !x.Completed).ToArray();

        public IActivityBatchValue ToBatch()
        {
            return this;
        }

        IActivityContextValue IActivityBatchValue.Context => Context;

        IEnumerable<IActivityValue> IActivityBatchValue.Activities => this.All();
    }
}