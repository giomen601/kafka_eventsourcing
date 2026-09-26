using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;

namespace Ticketing.Command.Infraestructure.EventSourcing;

public class TicketingEventSourcingHandler
(IEventStore eventStore)
: IEventSourcingHandler<TicketAggregate>
{
    public async Task<TicketAggregate> GetByIdAsync(string aggregateId, CancellationToken cancellationToken)
    {
        var aggregate = new TicketAggregate();
        var events = await eventStore.GetEventsAsync(aggregateId, cancellationToken);

        if (events is null || !events.Any())
            return aggregate;

        aggregate.ReplayEvents(events);

        aggregate.Version = events.Select(x => x.Version).Max();

        return aggregate;
    }

    public async Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken)
    {
        await eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommitChanges(), aggregate.Version, cancellationToken);

        aggregate.MarkChangesAsCommited();
    }
}
