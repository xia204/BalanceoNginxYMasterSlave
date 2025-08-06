﻿using System.ComponentModel.DataAnnotations;

namespace Uttt.Micro.Libro.Modelo
{
    public class LibreriaMaterial
    {
        [Key]
        public Guid LibreriaMateriaId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public DateTime? FechaPublicacion { get; set; }
        public Guid? AutorLibro { get; set; }
        public int NewData { get; set; }
    }
}
