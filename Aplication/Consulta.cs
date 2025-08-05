using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Uttt.Micro.Libro.Modelo;
using Uttt.Micro.Libro.Persistencia;


//Descomentar solo para hacer las migraciones
//Es el codigo original

namespace Uttt.Micro.Libro.Aplication
{
    public class Consulta
    {
        public class Ejecuta : IRequest<List<LibroMaterialDto>>
        {
            //Posible error eliminar si no funciona
            public Ejecuta()
            {

            }
        }
        public class Manejador : IRequestHandler<Ejecuta, List<LibroMaterialDto>>
        {

            private readonly ContextoLibreria _contexto;
            private readonly IMapper _mapper;

            public Manejador(ContextoLibreria contexto, IMapper mapper)
            {
                _mapper = mapper;
                _contexto = contexto;
            }

            public async Task<List<LibroMaterialDto>> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var libros = await _contexto.LibreriasMateriales.ToListAsync();
                var librosDto = _mapper.Map<List<LibreriaMaterial>, List<LibroMaterialDto>>(libros);
                return librosDto;
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
