using System.Collections;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Infraestructure.Repositories;

namespace Ticketing.Query.Infraestructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContextFactory contextFactory;
    private readonly TicketingDbContext context;
    private Hashtable repositories = new();
    private IEmployeeRepository? employeeRepository;

    public IEmployeeRepository EmployeeRepository
        => employeeRepository ??= new EmployeeRepository(context); 

    public UnitOfWork(DatabaseContextFactory contextFactory)
    {
        this.contextFactory = contextFactory;
        this.context = this.contextFactory.CreateContest();
    }
    public async Task<int> Complete()
    {
        return await context.SaveChangesAsync();
    }

    public IGenericRepository<TEntity> RepositoryGeneric<TEntity>() where TEntity : class
    {
        if (repositories is null)
            repositories = new Hashtable();

        var type = typeof(TEntity).Name;

        if (!repositories.Contains(type))
        {
            var repositoryType = typeof(GenericRepository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), context);
            repositories.Add(type, repositoryInstance);
        }

        return (IGenericRepository<TEntity>)repositories[type]!; // return one instance of the repository

    }
}