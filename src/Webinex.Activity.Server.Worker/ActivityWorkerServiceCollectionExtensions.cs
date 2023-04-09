using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity.Server.Worker
{
    public static class ActivityWorkerServiceCollectionExtensions
    {
        public static IActivityServerConfiguration AddWorker(
            [NotNull] this IActivityServerConfiguration serverConfiguration,
            Action<IActivityWorkerConfiguration> configure)
        {
            serverConfiguration = serverConfiguration ?? throw new ArgumentNullException(nameof(serverConfiguration));
            configure = configure ?? throw new ArgumentNullException(nameof(configure));

            var configuration = new ActivityWorkerConfiguration(serverConfiguration);
            configure(configuration);

            return serverConfiguration;
        }
    }
}