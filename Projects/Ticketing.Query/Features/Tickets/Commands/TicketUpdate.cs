using MediatR;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Domain.Tickets;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Features.Tickets.Commands;

public class TicketUpdate
{
    public record TicketUpdateCommand
    (
        string Id,
        int TicketType,
        string Description,
        string UserName
    )
    : IRequest<string>;

    public class TicketUpdateCommandHandler
    : IRequestHandler<TicketUpdateCommand, string>
    {
        private readonly IUnitOfWork unitOfWork;
        public TicketUpdateCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            
        }
        public async Task<string> Handle
        (
            TicketUpdateCommand command,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var ticket = await unitOfWork.RepositoryGeneric<Ticket>().GetByIdAsyng(new Guid(command.Id));

                if (ticket is null)
                {
                    throw new Exception("Tixket not found");
                }

                var employee = await unitOfWork.EmployeeRepository.GetByUserNameAsync(command.UserName);

                if (employee is null)
                {
                    employee = Employee.Create(string.Empty, string.Empty, null!, command.UserName);
                    unitOfWork.EmployeeRepository.AddEntity(employee);
                }

                ticket.Description = command.Description;
                ticket.TicketType = TicketType.Create(command.TicketType);
                
                await unitOfWork.Complete();

                return ticket.Id.ToString();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}