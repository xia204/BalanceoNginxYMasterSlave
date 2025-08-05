using AutoMapper;
using Uttt.Micro.Libro.Modelo;

namespace Uttt.Micro.Libro.Aplication
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LibreriaMaterial, LibroMaterialDto>();
        }
    }
}