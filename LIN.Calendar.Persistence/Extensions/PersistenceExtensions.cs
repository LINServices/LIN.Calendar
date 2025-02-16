using LIN.Calendar.Persistence.Context;
using LIN.Calendar.Persistence.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LIN.Calendar.Persistence.Extensions;

public static class PersistenceExtensions
{

    /// <summary>
    /// Agregar persistencia.
    /// </summary>
    /// <param name="services">Servicios.</param>
    /// <param name="configuration">Configuración.</param>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfigurationManager configuration)
    {

        services.AddDbContextPool<DataContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("somee"));
        });

        services.AddScoped<Events, Events>();
        services.AddScoped<Profiles, Profiles>();
        return services;

    }


    /// <summary>
    /// Utilizar persistencia.  
    /// </summary>
    public static IApplicationBuilder UsePersistence(this IApplicationBuilder app)
    {
        try
        {
            var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<DataContext>();
            context?.Database.EnsureCreated();
        }
        catch (Exception)
        {
        }
        return app;
    }

}