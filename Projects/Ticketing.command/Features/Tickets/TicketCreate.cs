using AutoMapper;
using Common.Core.Events;
using FluentValidation;
using MediatR;
using MongoDB.Driver;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;
using Ticketing.Command.Features.Apis;

namespace Ticketing.Command.Features.Tickets;

public sealed class TicketCreate : IMinimalApi
{

    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/api/ticket",
        async
        (
            TicketCreateRequest request,
            IMediator mediator,
            CancellationToken cancellationToken
        ) =>
        {
            string id = Guid.CreateVersion7(DateTimeOffset.UtcNow).ToString();
            TicketCreateCommand command = new(id, request);
            bool result = await mediator.Send(command);
            return Results.Ok(result);
        });
    }

    public sealed class TicketCreateRequest(string userName, int typeError, string detailError)
    {
        public string UserName { get; set; } = userName;
        public int TypeError { get; set; } = typeError;
        public string DetailError { get; set; } = detailError;
    }

    public record TicketCreateCommand
    (
        string id,
        TicketCreateRequest ticketCreateRequest
    )
    : IRequest<bool>;

    public class TicketCreateValidator : AbstractValidator<TicketCreateRequest>
    {
        public TicketCreateValidator()
        {
            RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username must not be emty")
            .EmailAddress()
            .WithMessage("Debe de ser de tipo email");

            RuleFor(x => x.TypeError)
            .NotEmpty()
            .WithMessage("Debe existir el tipo de error")
            .InclusiveBetween(1, 5)
            .WithMessage("el rango debe ir entre 1 a 5");

            RuleFor(x => x.DetailError)
            .NotEmpty()
            .WithMessage("DetailError must not be emty");
        }
    }

    public class TicketCreateCommandValidator : AbstractValidator<TicketCreateCommand>
    {
        public TicketCreateCommandValidator()
        {
            RuleFor(x => x.id)
            .NotEmpty().WithMessage("{PropertyName} no puede estar vacio, ingrese el Id del evento");

            RuleFor(x => x.ticketCreateRequest)
            .SetValidator(new TicketCreateValidator());
        }
    }

    public sealed class TicketCreateCommandHandler
    (
        IEventSourcingHandler<TicketAggregate> eventSourcingHandler
    )
    : IRequestHandler<TicketCreateCommand, bool>
    {
        public async Task<bool> Handle
        (
            TicketCreateCommand request,
            CancellationToken cancellationToken
        )
        {
            var aggregate = new TicketAggregate(request);
            await eventSourcingHandler.SaveAsync(aggregate, cancellationToken);
            return true;
        }
    }
}