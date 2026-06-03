using Azure.Core;

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
    }
}
