using Ticketing.Query.Domain.Tickets;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Features.Tickets.DTOS;

public static class TicketMapper
{
    public static TicketDto ToDto(this Ticket ticket)
    {
        return new
        (
            ticket.Id,
            ticket.Description!,
            ticket.TicketType!.Id
        );
    }
}

public record TicketDto(Guid Id, string Description, int TicketType);