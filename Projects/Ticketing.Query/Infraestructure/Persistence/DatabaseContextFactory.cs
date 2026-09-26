using Microsoft.EntityFrameworkCore;

namespace Ticketing.Query.Infraestructure.Persistence;

public class DatabaseContextFactory
{
    private readonly Action<DbContextOptionsBuilder> configureDbContext;

    public DatabaseContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
    {
        this.configureDbContext = configureDbContext;
    }

    public TicketingDbContext CreateContest()
    {
        DbContextOptionsBuilder<TicketingDbContext> optionsBuilder = new();
        configureDbContext(optionsBuilder);

        return new TicketingDbContext(optionsBuilder.Options);
    }
}