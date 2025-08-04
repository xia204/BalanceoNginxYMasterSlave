using Microsoft.EntityFrameworkCore;
using Uttt.Micro.Libro.Persistencia;

namespace Uttt.Micro.Libro.Interface
{
    public class ContextoLibreriaFactory : IContextoLibreriaFactory
    {
        private readonly IConfiguration _config;

        public ContextoLibreriaFactory(IConfiguration config)
        {
            _config = config;
        }

        public ContextoLibreria CreateDbContext(bool useMaster = false)
        {
            var connectionString = useMaster
                ? _config.GetConnectionString("MasterConnection")
                : _config.GetConnectionString("SlaveConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ContextoLibreria>();
            optionsBuilder.UseMySQL(connectionString);

            return new ContextoLibreria(optionsBuilder.Options);
        }
    }

}
