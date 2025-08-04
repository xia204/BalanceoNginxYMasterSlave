using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Uttt.Micro.Libro.Modelo;
using Uttt.Micro.Libro.Persistencia;

//Descomentar solo para hacer las migraciones
//Es el codigo original

namespace Uttt.Micro.Libro.Aplication
{
    public class ConsultaFiltro
    {
        public class LibroUnico : IRequest<LibroMaterialDto>
        {
            public Guid? LibroId { get; set; }
        }

        public class Manejador : IRequestHandler<LibroUnico, LibroMaterialDto>
        {
            private readonly ContextoLibreria _contexto;
            private readonly IMapper _mapper;
            public Manejador(ContextoLibreria contexto, IMapper mapper)
            {
                _contexto = contexto;
                _mapper = mapper;
            }
            public async Task<LibroMaterialDto> Handle(LibroUnico request, CancellationToken cancellationToken)
            {
                var libro = await _contexto.LibreriasMateriales
                    .Where(x => x.LibreriaMateriaId == request.LibroId).FirstOrDefaultAsync();
                if (libro == null)
                {
                    throw new Exception("No se encontro el libro");
                }

                var libroDto = _mapper.Map<LibreriaMaterial, LibroMaterialDto>(libro);
                return libroDto;
            }
        }
    }
}


//Código para busquedas en la base local y en la central
/*
namespace Uttt.Micro.Libro.Aplicacion
{
    public class ConsultaFiltro
    {
        public class LibroUnico : IRequest<LibroMaterialDto>
        {
            public Guid? LibroId { get; set; }
        }

        public class Manejador : IRequestHandler<LibroUnico, LibroMaterialDto>
        {
            private readonly ContextoLibreria _contexto;
            //Comentar el _slaveContext
            private readonly ContextoLibreriaReadOnly _slaveContext;
            private readonly IMapper _mapper;

            //Comentar esta linea para migraciones
            public Manejador(ContextoLibreria contexto, 
                ContextoLibreriaReadOnly slaveContext, 
                IMapper mapper)
            //Descomentar esta para migracion
            //public Manejador(ContextoLibreria masterContext, IMapper mapper)
            {
                _contexto = contexto;
                //comentar el _slaveContext
                _slaveContext = slaveContext;
                _mapper = mapper;
            }

            public async Task<LibroMaterialDto> Handle(LibroUnico request, CancellationToken cancellationToken)
            {
                // 1. Buscar primero en base local (master)
                var libro = await _contexto.LibreriasMateriales
                    .FirstOrDefaultAsync(x => x.LibreriaMateriaId == request.LibroId);

                // 2. Si no existe, buscar en base central (slave)
                //Comentar este if para migraciones
                if (libro == null)
                {
                    libro = await _slaveContext.LibreriasMateriales
                        .FirstOrDefaultAsync(x => x.LibreriaMateriaId == request.LibroId);
                }

                if (libro == null)
                    throw new Exception("No se encontró el libro ni en la base local ni en la central.");

                //return _mapper.Map<LibroMaterialDto>(libro);
                var libroDto = _mapper.Map<LibreriaMaterial, LibroMaterialDto>(libro);
                return libroDto;
            }

        }
    }
}
*/