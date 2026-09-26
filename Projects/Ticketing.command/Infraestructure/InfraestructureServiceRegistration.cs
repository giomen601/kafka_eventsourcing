using Common.Core.Events;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;
using Ticketing.Command.Infraestructure.EventSourcing;
using Ticketing.Command.Infraestructure.Persistence;
using Ticketing.Command.Infraestructure.Repositories;

namespace Ticketing.Command.Infraestructure;

public static class InfraestructureServiceRegistration
{
    public static IServiceCollection AddInfraestructureServices
    (
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        BsonClassMap.RegisterClassMap<BaseEvent>();
        BsonClassMap.RegisterClassMap<TicketCreatedEvent>();
        BsonClassMap.RegisterClassMap<TicketUpdatedEvent>();

        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>)); //scoped es uno solo para todo el ciclo de vida

        services.AddTransient<IEventModelRepository, EventModelRepository>(); //transient genera un nuevo objeto cada vez que se ejecuta el service

        services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient(configuration.GetConnectionString("MondoDb")));

        services.AddTransient<IEventStore, EventStore>();
        services.AddTransient<IEventSourcingHandler<TicketAggregate>, TicketingEventSourcingHandler>();

        services.AddScoped<IEventProducer, TicketEventProducer>();
        

        return services;
    }
}