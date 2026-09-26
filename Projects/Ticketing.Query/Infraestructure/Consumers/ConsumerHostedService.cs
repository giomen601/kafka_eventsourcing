
using System.Text.Json;
using Common.Core.Consumers;
using Common.Core.Events;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Infraestructure.Converters;

namespace Ticketing.Query.Infraestructure.Consumers;

public class ConsumerHostedService
: IHostedService
{
    private readonly ConsumerConfig config;
    private readonly ILogger<ConsumerHostedService> logger;
    private readonly IServiceProvider serviceProvider;

    public ConsumerHostedService
    (
        IOptions<ConsumerConfig> config,
        ILogger<ConsumerHostedService> logger,
        IServiceProvider serviceProvider
    )
    {
        this.config = config.Value;
        this.logger = logger;
        this.serviceProvider = serviceProvider;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Event consumer is working");
        var topic = "KAFKA_TOPIC";

        using (IServiceScope scope = serviceProvider.CreateAsyncScope())
        {
            var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();

            //Create new thread (to prevent api blocks)
            Task.Run(() => eventConsumer.Consume(topic), cancellationToken);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Services consumes stopped");
        return Task.CompletedTask;
    }
}