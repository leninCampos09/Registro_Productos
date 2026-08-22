using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Registro_Productos.Models
{
    public class Producto
    {
        [Key]
        public int idProducto { get; set; }

        [Required]
        [MaxLength(500)]
        public string producto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal precio { get; set; }

        public int cantidad { get; set; }

        public bool disponible { get; set; }

        [MaxLength(1000)]
        public string? img { get; set; }

        public string? descripcion { get; set; }

        public int? categoriaId { get; set; }

        public Categoria? Categoria { get; set; }
    }
}
