using Common.Core.Events;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Ticketing.Command.Application.Models;

namespace Ticketing.Command.Infraestructure.Persistence;

public class TicketEventProducer
: IEventProducer
{
    private readonly KafkaSettings kafkaSettings;

    public TicketEventProducer(IOptions<KafkaSettings> kafkaSettings)
    {
        this.kafkaSettings = kafkaSettings.Value;
    }
    public async Task ProduceAsync(string topic, BaseEvent @event)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = $"{kafkaSettings.Hostname}: {kafkaSettings.Port}"
        };

        using var producer = new ProducerBuilder<string, string>(config)
        .SetKeySerializer(Serializers.Utf8)
        .SetValueSerializer(Serializers.Utf8)
        .Build();

        var eventMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonConvert.SerializeObject(@event)
        };

        var deliveringStatus = await producer.ProduceAsync(topic, eventMessage);

        if (deliveringStatus.Status == PersistenceStatus.NotPersisted)
            throw new Exception($"no se pudo enviar el mensaje de tipo {@event.GetType().Name} hacia el topic {topic}, razón: {deliveringStatus.Message}");
    }
}