using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Activity.Server.Worker.ServiceBus
{
    public static class ServiceBusActivityWorkerConfigurationExtensions
    {
        public static IActivityWorkerConfiguration AddServiceBus(
            [NotNull] this IActivityWorkerConfiguration configuration,
            [NotNull] string connectionString,
            [NotNull] string topicPath = ActivityWorkerServiceBusDefaults.TopicPath,
            [NotNull] string subscriptionPath = ActivityWorkerServiceBusDefaults.SubscriptionPath)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            topicPath = topicPath ?? throw new ArgumentNullException(nameof(topicPath));
            subscriptionPath = subscriptionPath ?? throw new ArgumentNullException(nameof(subscriptionPath));

            var settings = new ActivityWorkerServiceBusSettings(connectionString, topicPath, subscriptionPath);
            configuration.ServerConfiguration.Services.AddSingleton(settings);
            configuration.ServerConfiguration.Services.AddHostedService<ActivityServiceBusBackgroundService>();

            return configuration;
        }
    }
}