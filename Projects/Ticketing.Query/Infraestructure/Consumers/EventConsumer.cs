using System.Text.Json;
using Common.Core.Consumers;
using Common.Core.Events;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Infraestructure.Converters;

namespace Ticketing.Query.Infraestructure.Consumers;

public class EventConsumer : IEventConsumer
{
    private readonly ConsumerConfig config;
    private readonly IServiceScopeFactory serviceProvider;

    public EventConsumer
    (
        IOptions<ConsumerConfig> config,
        IServiceScopeFactory serviceProvider
    )
    {
        this.config = config.Value;
        this.serviceProvider = serviceProvider;
    }
    public void Consume(string topic)
    {
        using var consumer = new ConsumerBuilder<string, string>(config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        consumer.Subscribe(topic);

        while (true)
        {
            var consumerResult = consumer.Consume();

            if (consumerResult is null)
                continue;

            if (consumerResult.Message is null)
                continue;

            var options = new JsonSerializerOptions
            {
                Converters = { new EventJsonConverter() }
            };

            var @event = JsonSerializer
                .Deserialize<BaseEvent>(consumerResult.Message.Value, options);

            if (@event is null)
                throw new ArgumentNullException("Message could't be processing");

            var scope = serviceProvider.CreateScope();

            var eventHandler = scope.ServiceProvider.GetRequiredService<IEventHandler>();

            var handlerMethod = eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });

            if (handlerMethod is null)
                throw new ArgumentNullException("Message could't be processing");

            handlerMethod.Invoke(eventHandler, new object[] { @event });

            //indicates if the event was consume to kafka
            consumer.Commit(consumerResult);
        }
    }
}