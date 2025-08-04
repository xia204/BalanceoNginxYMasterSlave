using Uttt.Micro.Libro.Persistencia;

namespace Uttt.Micro.Libro.Interface
{
    public interface IContextoLibreriaFactory
    {
        ContextoLibreria CreateDbContext(bool useMaster = false);
    }
}
