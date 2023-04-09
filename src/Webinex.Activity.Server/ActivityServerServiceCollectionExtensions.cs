using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Activity.Server
{
    public static class ActivityServerServiceCollectionExtensions
    {
        public static IServiceCollection AddActivityServer(
            [NotNull] this IServiceCollection services,
            [NotNull] Action<IActivityServerConfiguration> configure)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            configure = configure ?? throw new ArgumentNullException(nameof(configure));

            var configuration = new ActivityServerConfiguration(services);
            configure(configuration);

            return services;
        }
    }
}