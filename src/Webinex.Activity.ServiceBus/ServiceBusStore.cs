using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Webinex.Activity.ServiceBus
{
    internal class ServiceBusStore : IActivityStore
    {
        private readonly ActivityServiceBusSettings _settings;
        private readonly TopicClient _topicClient;

        public ServiceBusStore(ActivityServiceBusSettings settings)
        {
            _settings = settings;
            _topicClient = new TopicClient(settings.ConnectionString, settings.TopicPath, RetryPolicy.Default);
        }

        public Task StoreAsync(IActivityBatchValue batch)
        {
            var json = ActivityJson.Serialize(batch) ?? throw new ArgumentNullException();
            var body = Encoding.UTF8.GetBytes(json);
            var message = new Message(body)
            {
                MessageId = Guid.NewGuid().ToString(),
                UserProperties =
                {
                    ["OperationId"] = batch.Context.SystemValues.OperationId
                }
            };
            
            var task = _topicClient.SendAsync(message);

            return _settings.SendBehavior == ActivityServiceBusSendBehavior.Wait
                ? task
                : Task.CompletedTask;
        }
    }
}