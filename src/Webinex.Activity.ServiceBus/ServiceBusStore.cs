using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Webinex.Activity.ServiceBus
{
    internal class ServiceBusStore : IActivityStore, IAsyncDisposable
    {
        private readonly ActivityServiceBusSettings _settings;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;

        public ServiceBusStore(ActivityServiceBusSettings settings)
        {
            _settings = settings;
            _client = CreateClient();
            _sender = _client.CreateSender(settings.TopicPath);
        }

        public Task StoreAsync(IActivityBatchValue batch)
        {
            var json = ActivityJson.Serialize(batch) ?? throw new ArgumentNullException();
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(json))
            {
                MessageId = Guid.NewGuid().ToString()
            };
            message.ApplicationProperties["OperationId"] = batch.Context.SystemValues.OperationId;
            
            var task = _sender.SendMessageAsync(message);

            return _settings.SendBehavior == ActivityServiceBusSendBehavior.Wait
                ? task
                : Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await _sender.DisposeAsync();
            await _client.DisposeAsync();
        }

        private ServiceBusClient CreateClient()
        {
            return _settings.ConnectionString != null
                ? new ServiceBusClient(_settings.ConnectionString)
                : new ServiceBusClient(_settings.FullyQualifiedNamespace!, _settings.Credential!);
        }
    }
}
