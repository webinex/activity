using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
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
                await EnsureSubscriptionExistsAsync();
                Subscribe(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Fatal exception had happened on start");
                Environment.Exit(1251792);
            }
        }

        private async Task EnsureSubscriptionExistsAsync()
        {
            var management = new ManagementClient(_sbSettings.ConnectionString);
            if (await management.SubscriptionExistsAsync(_sbSettings.TopicPath, _sbSettings.SubscriptionPath))
            {
                await management.CloseAsync();
                return;
            }

            await management.CreateSubscriptionAsync(_sbSettings.TopicPath, _sbSettings.SubscriptionPath);
            await management.CloseAsync();
        }

        private void Subscribe(CancellationToken cancellationToken)
        {
            var subscription = new SubscriptionClient(
                _sbSettings.ConnectionString,
                _sbSettings.TopicPath,
                _sbSettings.SubscriptionPath)
            {
                OperationTimeout = TimeSpan.FromSeconds(15),
                PrefetchCount = 100,
            };

            cancellationToken.Register(() =>
            {
                subscription.CloseAsync().GetAwaiter().GetResult();
            });
            
            subscription.RegisterMessageHandler(HandleAsync, HandleExceptionAsync);
        }

        private async Task HandleAsync(Message message, CancellationToken token)
        {
            _logger.LogInformation("Message {Id} received", message.MessageId);

            await using var scope = _serviceProvider.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService<IActivityWorkerService>();
            var json = Encoding.UTF8.GetString(message.Body);
            var batch = ActivityJson.DeserializeBatch(json);
            await service.ProcessAsync(batch, token);

            _logger.LogInformation("Message {Id} proceed", message.MessageId);
        }

        private Task HandleExceptionAsync(ExceptionReceivedEventArgs args)
        {
            _logger.LogError(args.Exception, "Exception had happened");
            return Task.CompletedTask;
        }
    }
}