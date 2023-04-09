using System;

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

        public string ConnectionString { get; }
        
        public string TopicPath { get; }
        
        public ActivityServiceBusSendBehavior SendBehavior { get; }
    }
}