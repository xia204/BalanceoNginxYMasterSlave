using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Uttt.Micro.Libro.Aplication;
using Uttt.Micro.Libro.Persistencia;

namespace Uttt.Micro.Libro.Extensiones
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration, string writeConnection, string readConnection)
        {
            services.AddControllers();
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            services.AddDbContext<ContextoLibreria>(options =>
            {
                options.UseMySql(writeConnection, ServerVersion.AutoDetect(writeConnection));
            });

            services.AddDbContext<ContextoLibreriaReadOnly>(options =>
            {
                options.UseMySql(readConnection, ServerVersion.AutoDetect(readConnection));
            });

            services.AddMediatR(typeof(Nuevo.Manejador).Assembly);
            services.AddAutoMapper(typeof(Consulta.Manejador));
            return services;
        }
    }
}

