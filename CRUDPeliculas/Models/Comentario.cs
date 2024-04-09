namespace CRUDPeliculas.Entidades
{
    public class Comentario
    {
        public int Id { get; set; }

        public string Contenido { get; set; }

        public string UsuarioNombre { get; set; } // Propiedad para el nombre de usuario

        public int PeliculaId { get; set; }
        public Pelicula Pelicula { get; set; }
    }
}
