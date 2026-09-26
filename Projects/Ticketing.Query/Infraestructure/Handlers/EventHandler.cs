using Common.Core.Events;
using MediatR;
using Ticketing.Query.Domain.Abstractions;
using static Ticketing.Query.Features.Tickets.Commands.TicketCreate;
using static Ticketing.Query.Features.Tickets.Commands.TicketUpdate;


namespace Ticketing.Query.Infraestructure.Handlers;

public class EventHandler(IMediator mediator)
 : IEventHandler
{
    public async Task On(TicketCreatedEvent @event)
    {
        var command = new TicketCreateCommand
        (
            @event.Id,
            @event.UserName,
            @event.TypeError,
            @event.DetailError
        );

        await mediator.Send(command);
    }

    public async Task On(TicketUpdatedEvent @event)
    {
        var ticketUpdateCommand = new TicketUpdateCommand
        (
            @event.Id,
            @event.TicketType!.Value,
            @event.Description!,
            @event.Username!
        );

        await mediator.Send(ticketUpdateCommand);
    }
}