using System.ComponentModel.DataAnnotations;

namespace CRUDPeliculas.Models
{
    public class RegistroViewModel
    {

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "El campo debe ser u  Email válido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
