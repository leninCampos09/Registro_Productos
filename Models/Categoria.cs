using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Registro_Productos.Models
{
    public class Categoria
    {
        [Key]
        public int idCategoria { get; set; }

        [Required]
        [MaxLength(200)]
        public string nombre { get; set; }

        public ICollection<Producto>? Productos { get; set; }
    }
}
