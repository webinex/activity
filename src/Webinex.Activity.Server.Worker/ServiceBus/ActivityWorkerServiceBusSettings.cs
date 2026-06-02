using Azure.Core;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace Webinex.Activity.Server.Worker.ServiceBus
{
    internal class ActivityWorkerServiceBusSettings
    {
        public ActivityWorkerServiceBusSettings(string connectionString, string topicPath, string subscriptionPath)
        {
            ConnectionString = connectionString;
            TopicPath = topicPath;
            SubscriptionPath = subscriptionPath;
        }

        public ActivityWorkerServiceBusSettings(
            string fullyQualifiedNamespace,
            TokenCredential credential,
            string topicPath,
            string subscriptionPath)
        {
            FullyQualifiedNamespace = fullyQualifiedNamespace;
            Credential = credential;
            TopicPath = topicPath;
            SubscriptionPath = subscriptionPath;
        }

        public string? ConnectionString { get; }
        public string? FullyQualifiedNamespace { get; }
        public TokenCredential? Credential { get; }
        public string TopicPath { get; }
        public string SubscriptionPath { get; }

        public ServiceBusClient CreateClient()
        {
            return ConnectionString != null
                ? new ServiceBusClient(ConnectionString)
                : new ServiceBusClient(FullyQualifiedNamespace!, Credential!);
        }

        public ServiceBusAdministrationClient CreateAdministrationClient()
        {
            return ConnectionString != null
                ? new ServiceBusAdministrationClient(ConnectionString)
                : new ServiceBusAdministrationClient(FullyQualifiedNamespace!, Credential!);
        }
    }
}
