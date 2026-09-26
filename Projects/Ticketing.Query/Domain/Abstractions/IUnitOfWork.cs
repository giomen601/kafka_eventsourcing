using Ticketing.Query.Domain.Employees;

namespace Ticketing.Query.Domain.Abstractions;

public interface IUnitOfWork
{
    IGenericRepository<TEntity> RepositoryGeneric<TEntity>() where TEntity : class;
    IEmployeeRepository EmployeeRepository { get; }
    Task<int> Complete();
}