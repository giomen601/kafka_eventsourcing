using System.Reflection;

namespace Ticketing.Query.Application;
public static class ApplicationServiceRegistration
{
    public static IServiceCollection RegisterAppicationServices
    (
        this IServiceCollection services
    )
    {
        var currentAssebly = Assembly.GetExecutingAssembly();

        services.AddMediatR(m =>
        {
            m.RegisterServicesFromAssemblies(currentAssebly);
        });
        
        return services;
    }
}