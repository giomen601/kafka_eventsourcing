using Common.Core.Consumers;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Infraestructure.Consumers;
using Ticketing.Query.Infraestructure.Persistence;
using Ticketing.Query.Infraestructure.Persistence.Interceptors;
using Ticketing.Query.Infraestructure.Repositories;

namespace Ticketing.Query.Infraestructure;
public static class InfraestructureServiceRegistration
{
     public static IServiceCollection RegisterInfraestructureServices
     (
          this IServiceCollection services,
          IConfiguration configuration
     )
     {
          Action<DbContextOptionsBuilder> configureDbContest;

          services.AddSingleton<AuditEntitiesInterceptor>();

          string connectionString = configuration.GetConnectionString("PostgresConnectionString")
                                   ?? throw new ArgumentException(nameof(configuration));

          configureDbContest = o => o
          .UseLazyLoadingProxies()
          .UseNpgsql(connectionString)
          .UseSnakeCaseNamingConvention()
          .AddInterceptors(new AuditEntitiesInterceptor());

          //services.AddDbContext<TicketingDbContext>(configureDbContest);

          services.AddDbContext<TicketingDbContext>(opt =>
          {
          opt.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
          });

          services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDbContest));
          services.AddScoped<IUnitOfWork, UnitOfWork>();
          services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

          services.AddScoped<IEmployeeRepository, EmployeeRepository>();

          services.AddScoped<IEventConsumer, EventConsumer>();

          services.AddHostedService<ConsumerHostedService>();

          services.AddScoped<IEventHandler, Handlers.EventHandler>();

          services.Configure<ConsumerConfig>(configuration.GetSection(nameof(ConsumerConfig)));

          return services;
     }
   
    
}