using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Webinex.Activity
{
    public interface IActivitySettings
    {
        [MaybeNull]
        string ForwardingSecret { get; }
    }
    
    public interface IActivityConfiguration
    {
        IServiceCollection Services { get; }
        
        IDictionary<string, object> Values { get; }

        IActivityConfiguration UseForwardingSecret([NotNull] string secret);
    }
    
    internal class ActivityConfiguration : IActivityConfiguration, IActivitySettings
    {
        private ActivityConfiguration(IServiceCollection services)
        {
            Services = services;

            services.AddSingleton<IActivitySettings>(this);
            services.TryAddSingleton<IActivityIncomeContext, NullActivityIncomeContext>();
            services.AddSingleton<ActivityScopeAccessor>();
            services.AddSingleton<IActivityScopeAccessor>(x => x.GetRequiredService<ActivityScopeAccessor>());
            services.AddSingleton<IActivityScopeProvider>(x => x.GetRequiredService<ActivityScopeAccessor>());
            services.AddSingleton<IActivityScope>(x => x.GetRequiredService<ActivityScopeAccessor>());
            services.AddScoped<IActivityScopeFactory, ActivityScopeFactory>();
        }

        public IActivityConfiguration UseForwardingSecret(string secret)
        {
            ForwardingSecret = secret ?? throw new ArgumentNullException(nameof(secret));
            return this;
        }

        public IServiceCollection Services { get; }

        public IDictionary<string, object> Values { get; } = new Dictionary<string, object>();

        public string ForwardingSecret { get; private set; }

        public static ActivityConfiguration GetOrCreate(IServiceCollection services)
        {
            var instance = (ActivityConfiguration)
                services.FirstOrDefault(x =>
                    x.ServiceType == typeof(ActivityConfiguration))?.ImplementationInstance;

            if (instance != null)
                return instance;

            instance = new ActivityConfiguration(services);
            services.AddSingleton(instance);
            return instance;
        }
    }
}