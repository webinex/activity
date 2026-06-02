using System;
using System.Diagnostics.CodeAnalysis;
using Azure.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Activity.ServiceBus
{
    public static class ActivityConfigurationExtensions
    {
        public static IActivityConfiguration AddServiceBusStore(
            [NotNull] this IActivityConfiguration configuration,
            [NotNull] string connectionString,
            [NotNull] string topicPath = "activity",
            ActivityServiceBusSendBehavior sendBehavior = ActivityServiceBusSendBehavior.Wait)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            topicPath = topicPath ?? throw new ArgumentNullException(nameof(topicPath));

            var settings = new ActivityServiceBusSettings(connectionString, topicPath, sendBehavior);
            configuration.Services.AddSingleton(settings);
            configuration.Services.AddSingleton<IActivityStore, ServiceBusStore>();

            return configuration;
        }

        public static IActivityConfiguration AddServiceBusStore(
            [NotNull] this IActivityConfiguration configuration,
            [NotNull] string fullyQualifiedNamespace,
            [NotNull] TokenCredential credential,
            [NotNull] string topicPath = "activity",
            ActivityServiceBusSendBehavior sendBehavior = ActivityServiceBusSendBehavior.Wait)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            fullyQualifiedNamespace = fullyQualifiedNamespace ??
                                      throw new ArgumentNullException(nameof(fullyQualifiedNamespace));
            credential = credential ?? throw new ArgumentNullException(nameof(credential));
            topicPath = topicPath ?? throw new ArgumentNullException(nameof(topicPath));

            var settings = new ActivityServiceBusSettings(
                fullyQualifiedNamespace,
                credential,
                topicPath,
                sendBehavior);
            configuration.Services.AddSingleton(settings);
            configuration.Services.AddSingleton<IActivityStore, ServiceBusStore>();

            return configuration;
        }
    }
}
