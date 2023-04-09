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

        public string ConnectionString { get; }
        public string TopicPath { get; }
        public string SubscriptionPath { get; }
    }
}