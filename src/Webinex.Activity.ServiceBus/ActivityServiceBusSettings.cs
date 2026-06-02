using System;

using Azure.Core;
using Azure.Messaging.ServiceBus;

namespace Webinex.Activity.ServiceBus
{
    internal class ActivityServiceBusSettings
    {
        public ActivityServiceBusSettings(string connectionString, string topicPath, ActivityServiceBusSendBehavior sendBehavior)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            TopicPath = topicPath ?? throw new ArgumentNullException(nameof(topicPath));
            SendBehavior = sendBehavior;
        }

        public ActivityServiceBusSettings(
            string fullyQualifiedNamespace,
            TokenCredential credential,
            string topicPath,
            ActivityServiceBusSendBehavior sendBehavior)
        {
            FullyQualifiedNamespace = fullyQualifiedNamespace ??
                                      throw new ArgumentNullException(nameof(fullyQualifiedNamespace));
            Credential = credential ?? throw new ArgumentNullException(nameof(credential));
            TopicPath = topicPath ?? throw new ArgumentNullException(nameof(topicPath));
            SendBehavior = sendBehavior;
        }

        public string? ConnectionString { get; }

        public string? FullyQualifiedNamespace { get; }

        public TokenCredential? Credential { get; }
        
        public string TopicPath { get; }
        
        public ActivityServiceBusSendBehavior SendBehavior { get; }

        public ServiceBusClient CreateClient()
        {
            return ConnectionString != null
                ? new ServiceBusClient(ConnectionString)
                : new ServiceBusClient(FullyQualifiedNamespace!, Credential!);
        }
    }
}
