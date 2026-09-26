using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Infraestructure.Persistence;

namespace Ticketing.Query.Infraestructure.Repositories;

public class EmployeeRepository
: GenericRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(TicketingDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Employee?> GetByUserNameAsync(string UserName)
    {
        return await _dbContext.Employees
        .Where(x => x.FirstName == UserName)
        .FirstOrDefaultAsync();
    }
}