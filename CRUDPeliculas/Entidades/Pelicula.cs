using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRUDPeliculas.Entidades
{
    public class Pelicula
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(250, ErrorMessage = "El título no puede tener más de 250 caracteres.")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El director es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del director no puede tener más de 100 caracteres.")]
        public string Director { get; set; }

        [Required(ErrorMessage = "El año es obligatorio.")]
        [Range(1900, 2100, ErrorMessage = "Por favor, introduzca un año válido.")]
        public int Anio { get; set; }

        [StringLength(500, ErrorMessage = "La sinopsis no puede tener más de 500 caracteres.")]
        public string Sinopsis { get; set; }

        [Url(ErrorMessage = "La URL de la imagen no es válida.")]
        public string ImagenUrl { get; set; }

        [Required(ErrorMessage = "Por favor, seleccione un género.")]
        public string Genero { get; set; }

        // Relación uno a muchos con Comentario
        public ICollection<Comentario> Comentarios { get; set; }
    }
}
