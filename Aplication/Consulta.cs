using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Uttt.Micro.Libro.Modelo;
using Uttt.Micro.Libro.Persistencia;


namespace Uttt.Micro.Libro.Aplication
{
    public class Consulta
    {
        public class Ejecuta : IRequest<List<LibroMaterialDto>>
        {
            public Ejecuta()
            {

            }
        }
        public class Manejador : IRequestHandler<Ejecuta, List<LibroMaterialDto>>
        {
            private readonly ContextoLibreria _contexto;
            private readonly ContextoLibreriaReadOnly _slaveContext;
            private readonly IMapper _mapper;

            public Manejador(ContextoLibreria contexto, ContextoLibreriaReadOnly slaveContext, IMapper mapper)
            {
                _contexto = contexto;
                _slaveContext = slaveContext;
                _mapper = mapper;
            }

            public async Task<List<LibroMaterialDto>> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                // 1. Consultar en base local (master)
                var librosMaster = await _contexto.LibreriasMateriales.ToListAsync();

                // 2. Consultar en base slave
                var librosSlave = await _slaveContext.LibreriasMateriales.ToListAsync();

                // 3. Combinar resultados de ambas bases de datos
                var todosLosLibros = new List<LibreriaMaterial>();
                todosLosLibros.AddRange(librosMaster);
                todosLosLibros.AddRange(librosSlave);

                // 4. Eliminar duplicados por ID
                var librosUnicos = todosLosLibros
                    .GroupBy(x => x.LibreriaMateriaId)
                    .Select(g => g.First())
                    .ToList();

                return _mapper.Map<List<LibroMaterialDto>>(librosUnicos);
            }
        }
    }
}


//Código para busquedas en la base local y en la central
/*
namespace Uttt.Micro.Libro.Aplicacion
{
    public class Consulta
    {
        public class Ejecuta : IRequest<List<LibroMaterialDto>>
        {
            public Ejecuta()
            {

            }
        }
        public class Manejador : IRequestHandler<Ejecuta, List<LibroMaterialDto>>
        {
            private readonly ContextoLibreria _contexto;
            //comentar igual el slaveContext
            private readonly ContextoLibreriaReadOnly _slaveContext;
            private readonly IMapper _mapper;

            //Comentar esta linea para migraciones
            public Manejador(ContextoLibreria contexto, ContextoLibreriaReadOnly slaveContext, IMapper mapper)
            //Descomentar esta linea para migraciones
            //public Manejador(ContextoLibreria masterContext, IMapper mapper)
            {
                _contexto = contexto;
                //Comentar el slave para las migraciones
                _slaveContext = slaveContext;
                _mapper = mapper;
            }

            public async Task<List<LibroMaterialDto>> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                // 1. Consultar en base local (master)
                var libros = await _contexto.LibreriasMateriales.ToListAsync();

                // 2. Si está vacía, consultar en base central (slave)
                //Comentar este if para migraciones
                if (libros == null || !libros.Any())
                {
                    libros = await _slaveContext.LibreriasMateriales.ToListAsync();
                }

                return _mapper.Map<List<LibroMaterialDto>>(libros);
            }
        }
    }
}*/
