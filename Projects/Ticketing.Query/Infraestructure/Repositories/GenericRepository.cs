using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Infraestructure.Persistence;

namespace Ticketing.Query.Infraestructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly TicketingDbContext _dbContext;

    public GenericRepository(TicketingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public void AddEntity(T Entity)
    {
        _dbContext.Set<T>().Add(Entity);
    }

    public void DeleteEntity(T Entity)
    {
        _dbContext.Set<T>().Remove(Entity);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public async Task<T?> GetByIdAsyng(Guid id)
    {
        Console.WriteLine("GetByIdAsyng ======================================>");
        var result = await _dbContext.Set<T>().FindAsync(id);
        Console.WriteLine("EXIT GetByIdAsyng ======================================>");
        return result;
    }

    public void UpdateEntity(T Entity)
    {
        _dbContext.Set<T>().Attach(Entity);
        _dbContext.Entry(Entity).State = EntityState.Modified;
    }
}