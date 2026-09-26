using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Infraestructure.Persistence;

namespace Ticketing.Query.Application.Extensions;
public static class MigrationExtension
{
    public static async Task ApplyMigration(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateAsyncScope())
        {
            var service = scope.ServiceProvider;
            var loggerFactory = service.GetRequiredService<ILoggerFactory>();

            try
            {
                var contextFactory = service.GetRequiredService<DatabaseContextFactory>();
                using TicketingDbContext dbContext = contextFactory.CreateContest();
                await dbContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "error en la migración");
            }
        }
    }
}