using MediatR;
using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Features.Tickets.DTOS;
using Ticketing.Query.Infraestructure.Persistence;

namespace Ticketing.Query.Features.Tickets.Queries;

public class TicketGet
{
    public class TicketGetQuery : IRequest<List<TicketDto>>
    {

    }

    public class TicketGetQueryHandler(TicketingDbContext ticketingDbContext)
    : IRequestHandler<TicketGetQuery, List<TicketDto>>
    {
        public async Task<List<TicketDto>> Handle
        (
            TicketGetQuery request,
            CancellationToken cancellationToken
        )
        {
            var tickets = await ticketingDbContext.Tickets.ToListAsync();

            return tickets.ConvertAll(x => x.ToDto());
        }
    }
}