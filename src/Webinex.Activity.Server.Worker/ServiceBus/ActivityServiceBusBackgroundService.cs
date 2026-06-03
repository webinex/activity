using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Webinex.Activity.Server.Worker.ServiceBus
{
    internal class ActivityServiceBusBackgroundService : BackgroundService
    {
        private readonly ILogger<ActivityServiceBusBackgroundService> _logger;
        private readonly ActivityWorkerServiceBusSettings _sbSettings;
        private readonly IServiceProvider _serviceProvider;

        public ActivityServiceBusBackgroundService(
            ILogger<ActivityServiceBusBackgroundService> logger,
            ActivityWorkerServiceBusSettings sbSettings,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _sbSettings = sbSettings;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await EnsureSubscriptionExistsAsync(stoppingToken);
                await SubscribeAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Fatal exception had happened on start");
                Environment.Exit(1251792);
            }
        }

        private async Task EnsureSubscriptionExistsAsync(CancellationToken cancellationToken)
        {
            var management = CreateAdministrationClient();
            if (await management.SubscriptionExistsAsync(
                    _sbSettings.TopicPath,
                    _sbSettings.SubscriptionPath,
                    cancellationToken))
            {
                return;
            }

            await management.CreateSubscriptionAsync(
                _sbSettings.TopicPath,
                _sbSettings.SubscriptionPath,
                cancellationToken);
        }

        private async Task SubscribeAsync(CancellationToken cancellationToken)
        {
            await using var client = CreateClient();
            await using var processor = client.CreateProcessor(
                _sbSettings.TopicPath,
                _sbSettings.SubscriptionPath,
                new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = true,
                    MaxConcurrentCalls = 1,
                    PrefetchCount = 100,
                });

            processor.ProcessMessageAsync += HandleAsync;
            processor.ProcessErrorAsync += HandleExceptionAsync;

            await processor.StartProcessingAsync(cancellationToken);
            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            }
            finally
            {
                await processor.StopProcessingAsync(CancellationToken.None);
            }
        }

        private async Task HandleAsync(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            _logger.LogInformation("Message {Id} received", message.MessageId);

            await using var scope = _serviceProvider.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService<IActivityWorkerService>();
            var batch = ActivityJson.DeserializeBatch(message.Body.ToString());
            await service.ProcessAsync(batch, args.CancellationToken);

            _logger.LogInformation("Message {Id} proceed", message.MessageId);
        }

        private Task HandleExceptionAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(
                args.Exception,
                "Exception had happened while processing {EntityPath} from {ErrorSource}",
                args.EntityPath,
                args.ErrorSource);
            return Task.CompletedTask;
        }

        private ServiceBusClient CreateClient()
        {
            return _sbSettings.ConnectionString != null
                ? new ServiceBusClient(_sbSettings.ConnectionString)
                : new ServiceBusClient(_sbSettings.FullyQualifiedNamespace!, _sbSettings.Credential!);
        }

        private ServiceBusAdministrationClient CreateAdministrationClient()
        {
            return _sbSettings.ConnectionString != null
                ? new ServiceBusAdministrationClient(_sbSettings.ConnectionString)
                : new ServiceBusAdministrationClient(_sbSettings.FullyQualifiedNamespace!, _sbSettings.Credential!);
        }
    }
}
