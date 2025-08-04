using Microsoft.EntityFrameworkCore;
using Uttt.Micro.Libro.Modelo;

namespace Uttt.Micro.Libro.Persistencia
{
    public class ContextoLibreriaReadOnly : DbContext
    {
        public ContextoLibreriaReadOnly(DbContextOptions<ContextoLibreriaReadOnly> options) : base(options)
        {
        }

        public DbSet<LibreriaMaterial> LibreriasMateriales { get; set; }
    }
}
