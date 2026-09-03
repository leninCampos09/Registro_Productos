using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Registro_Productos.Models
{
    public class Venta
    {
        [Key]
        public int idVenta { get; set; }

        public DateTime fecha { get; set; }

        public int productoId { get; set; }

        public int cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal total { get; set; }

        [MaxLength(250)]
        public string? clienteNombre { get; set; }

        [MaxLength(50)]
        public string? clienteTelefono { get; set; }

        [MaxLength(200)]
        public string? clienteEmail { get; set; }

        // navegación opcional
        public Producto? Producto { get; set; }
    }
}
