using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Uttt.Micro.Libro.Aplication;
using Uttt.Micro.Libro.Persistencia;

namespace Uttt.Micro.Libro.Extensiones
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers()
                .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Nuevo>());

            //Descomentar para ejecutar las migraciones 
            /*services.AddDbContext<ContextoLibreria>(options =>
            {
                options.UseMySQL(configuration.GetConnectionString("DefaultConnection"));
            });*/

            //Comentar estos dos servicios y eliminar u omitir el archivo ContextoLibreriaReadOnly
            services.AddDbContext<ContextoLibreria>(options =>
            {
                options.UseMySQL(configuration.GetConnectionString("DefaultConnection"));
                Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddDbContext<ContextoLibreriaReadOnly>(options =>
            {
                options.UseMySQL(configuration.GetConnectionString("SlaveConnection"));
                Console.WriteLine(configuration.GetConnectionString("SlaveConnection"));

            });


            services.AddMediatR(typeof(Nuevo.Manejador).Assembly);
            services.AddAutoMapper(typeof(Consulta.Manejador));

            return services;
        }
    }
}
