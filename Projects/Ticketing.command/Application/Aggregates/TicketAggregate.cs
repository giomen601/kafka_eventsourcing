using Common.Core.Events;
using Ticketing.Command.Domain.Abstracts;
using static Ticketing.Command.Features.Tickets.TicketCreate;

namespace Ticketing.Command.Application.Aggregates;

public class TicketAggregate : AggregateRoot
{
    public bool Active { get; set; }
    public TicketAggregate()
    {

    }

    public TicketAggregate(TicketCreateCommand command)
    {
        TicketCreatedEvent ticketCreateEvent = new()
        {
            Id = command.id,
            UserName = command.ticketCreateRequest.UserName,
            TypeError = command.ticketCreateRequest.TypeError,
            DetailError = command.ticketCreateRequest.DetailError
        };

        RaiseEvent(ticketCreateEvent);
    }

    public void Apply(TicketCreatedEvent @event)
    {
        _id = @event.Id;
        Active = true;
    }

    public void EditTicket(int ticketType, string description, string userName)
    {
        if (!Active)
            throw new InvalidOperationException("No se puede editar el ticket inactivo");

        RaiseEvent(new TicketUpdatedEvent
        {
            Id = Id,
            TicketType = ticketType,
            Description = description,
            Username = userName
        });
    }

    public void Apply(TicketUpdatedEvent @event)
    {
        _id = @event.Id;
    }
}