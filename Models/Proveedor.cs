using System.ComponentModel.DataAnnotations;

namespace Registro_Productos.Models
{
    public class Proveedor
    {
        [Key]
        public int idProveedor { get; set; }

        [Required]
        [MaxLength(250)]
        public string nombre { get; set; }

        [MaxLength(50)]
        public string? telefono { get; set; }

        [MaxLength(200)]
        public string? email { get; set; }

        [MaxLength(500)]
        public string? direccion { get; set; }
    }
}
