using FluentValidation;
using MediatR;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Features.Apis;

namespace Ticketing.command.Features.Tickets;

public class TicketUpdate : IMinimalApi
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPut("/api/ticket/{id}",
        async
        (
            string id,
            TicketUpdateRequest request,
            IMediator mediator,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new TicketUpdateCommand(id, request);
            var result =  await mediator.Send(command);
            return Results.Ok(result);
        }).WithName("UpdateTicket");
    }

    public sealed class TicketUpdateRequest
    (int ticketType, string description, string userName)
    {
        public int TicketType { get; } = ticketType;
        public string Description { get; } = description;
        public string UserName { get; } = userName;
    }

    public record TicketUpdateCommand(string Id, TicketUpdateRequest Request) : IRequest<bool>;

    public class TicketUpdateCommandValidator : AbstractValidator<TicketUpdateCommand>
    {
        public TicketUpdateCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Request)
            .SetValidator(new TicketUpdateRequestValidator());
        }
    }

    public class TicketUpdateRequestValidator
    : AbstractValidator<TicketUpdateRequest>
    {
        public TicketUpdateRequestValidator()
        {
            RuleFor(x => x.TicketType)
            .NotEmpty().WithMessage("{PropertyName} couldn't be empty")
            .InclusiveBetween(1, 5).WithMessage("{PropertyName} must be between 1 to 5");

            RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Insert description");

            RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Insert UserName");
        }
    }

    public sealed class TicketUpdateCommandHandler
    (IEventSourcingHandler<TicketAggregate> eventSourcingHandler)
    : IRequestHandler<TicketUpdateCommand, bool>
    {
        public async Task<bool> Handle
        (
            TicketUpdateCommand command,
            CancellationToken cancellationToken
        )
        {
            var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id, cancellationToken);

            aggregate.EditTicket(command.Request.TicketType, command.Request.Description, command.Request.UserName);

            await eventSourcingHandler.SaveAsync(aggregate, cancellationToken);

            return true;
        }
    }
}

