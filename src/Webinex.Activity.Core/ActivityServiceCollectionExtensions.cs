using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Activity
{
    public static class ActivityServiceCollectionExtensions
    {
        public static IServiceCollection AddActivity(
            [NotNull] this IServiceCollection services,
            Action<IActivityConfiguration> configure = null)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            var configuration = ActivityConfiguration.GetOrCreate(services);
            configure?.Invoke(configuration);

            return services;
        }
    }
}