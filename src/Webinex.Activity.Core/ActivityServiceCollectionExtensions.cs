using System;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Activity
{
    public static class ActivityServiceCollectionExtensions
    {
        public static IServiceCollection AddActivity(
            this IServiceCollection services,
            Action<IActivityConfiguration>? configure = null)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            var configuration = ActivityConfiguration.GetOrCreate(services);
            configure?.Invoke(configuration);

            return services;
        }
    }
}