using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Query.Features.Tickets.DTOS;
using Ticketing.Query.Features.Tickets.Queries;
using static Ticketing.Query.Features.Tickets.Queries.TicketGet;

namespace Ticketing.Query.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketController : ControllerBase
{
    private readonly IMediator mediator;

    public TicketController
    (
        IMediator mediator
    )
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<TicketDto>>> Get()
    {
        var query = new TicketGetQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }
}