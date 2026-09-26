using MediatR;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Domain.Tickets;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Features.Tickets.Commands;

public sealed class TicketCreate
{
    public record TicketCreateCommand(string Id, string UserName, int TicketType, string DetailError)
    : IRequest<string>;

    public class TicketCreateCommandHandler
    (
        IUnitOfWork unitOfWork
    )
    : IRequestHandler<TicketCreateCommand, string>
    {
        public async Task<string> Handle(TicketCreateCommand request, CancellationToken cancellationToken)
        {
            //1. insert employee data
            var employee = await unitOfWork.EmployeeRepository.GetByUserNameAsync(request.UserName);

            if (employee is null)
            {
                employee = Employee.Create(string.Empty, string.Empty, null!, request.UserName);
                unitOfWork.EmployeeRepository.AddEntity(employee);
            }

            //2. insert ticket data
            Ticket ticket = Ticket.Create(new Guid(request.Id), TicketType.Create(request.TicketType), request.DetailError);
            unitOfWork.RepositoryGeneric<Ticket>().AddEntity(ticket);

            //3. insert ticketemployee relation
            TicketEmployee ticketEmployee = TicketEmployee.Create(ticket, employee);
            unitOfWork.RepositoryGeneric<TicketEmployee>().AddEntity(ticketEmployee);

            //Insert in database
            await unitOfWork.Complete();

            return ticket.Id.ToString();
        }
    }
}