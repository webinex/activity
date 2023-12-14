using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Webinex.Activity.Http;

namespace Webinex.Activity
{
    public interface IActivityScopeFactory
    {
        IActivityScope Create(IActivitySystemValues? systemValues, ActivityPathItem[]? path);
        IActivityScope Create(ActivityToken? token = null);
    }

    internal class ActivityScopeFactory : IActivityScopeFactory
    {
        private readonly IActivityStore _activityStore;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IEnumerable<IActivityContextInitializer> _contextInitializers;

        public ActivityScopeFactory(
            IActivityStore activityStore,
            ILoggerFactory loggerFactory,
            IEnumerable<IActivityContextInitializer> contextInitializers)
        {
            _activityStore = activityStore;
            _loggerFactory = loggerFactory;
            _contextInitializers = contextInitializers;
        }

        public IActivityScope Create(IActivitySystemValues? systemValues, ActivityPathItem[]? path)
        {
            return new ActivityScope(
                _activityStore,
                _loggerFactory,
                _contextInitializers,
                new ConstantActivityIncomeContext(systemValues, path));
        }

        public IActivityScope Create(ActivityToken? token = null)
        {
            return new ActivityScope(
                _activityStore,
                _loggerFactory,
                _contextInitializers,
                new ConstantActivityIncomeContext(token?.SystemValues, token?.Path));
        }

        private class ConstantActivityIncomeContext : IActivityIncomeContext
        {
            public ConstantActivityIncomeContext(
                IActivitySystemValues? systemValues,
                ActivityPathItem[]? path)
            {
                SystemValues = systemValues;
                Path = path ?? Array.Empty<ActivityPathItem>();
            }

            public IActivitySystemValues? SystemValues { get; }
            public ActivityPathItem[] Path { get; }
        }
    }
}