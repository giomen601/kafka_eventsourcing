using Common.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Ticketing.Command.Application.Models;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;

namespace Ticketing.Command.Infraestructure.Persistence;

public class EventStore
: IEventStore
{
    private readonly IEventModelRepository eventModelRepository;
    private readonly IEventProducer eventProducer;
    private readonly KafkaSettings kafkaSettings;

    public EventStore
    (
        IEventModelRepository eventModelRepository,
        IOptions<KafkaSettings> kafkaSettings,
        IEventProducer eventProducer
    )
    {
        this.eventModelRepository = eventModelRepository;
        this.eventProducer = eventProducer;
        this.kafkaSettings = kafkaSettings.Value;
    }

    public async Task<List<BaseEvent>> GetEventsAsync(string aggregateId, CancellationToken cancellationToken)
    {
        var eventStream = await eventModelRepository
        .FilterByAsync(x => x.AggregateIdentifier == aggregateId, cancellationToken);

        if (eventStream == null || !eventStream.Any())
            throw new Exception("El aggregate no tiene eventos");

        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList()!;
    }

    public async Task SaveEventsAsync(string aggregateId, IEnumerable<BaseEvent> events, int expectedVersion, CancellationToken cancellationToken)
    {
        var eventStream = await eventModelRepository
        .FilterByAsync(x => x.AggregateIdentifier == aggregateId, cancellationToken);

        if (eventStream.Any() && expectedVersion != 1 && eventStream.Last().Version != expectedVersion)
            throw new Exception("Error de concurrencia");

        var version = expectedVersion;

        foreach (var @event in events)
        {
            version++;
            @event.Version = version;
            var eventType = @event.GetType().Name;
            var eventModel = new EventModel
            {
                Timestamp = DateTime.UtcNow,
                AggregateIdentifier = aggregateId,
                Version = version,
                EventType = eventType,
                EventData = @event
            };
            await AddEventStore(eventModel, cancellationToken);

            var topic = kafkaSettings.Topic ?? throw new Exception("Topic no encontrado");

            await eventProducer.ProduceAsync(topic, @event);
        }
    }

    private async Task AddEventStore(EventModel @event, CancellationToken cancellationToken)
    {
        IClientSessionHandle session = await eventModelRepository.BeginSessionAsync(cancellationToken);

        try
        {
            eventModelRepository.BeginTransaction(session);
            await eventModelRepository.InsertOneAsync(@event, session, cancellationToken);

            await eventModelRepository.CommitTransactionAsync(session, cancellationToken);
            eventModelRepository.DisposeSession(session);
        }
        catch (Exception ex)
        {
            await eventModelRepository.RollbackTrasactionAsync(session, cancellationToken);
            eventModelRepository.DisposeSession(session);
        }
    }
}