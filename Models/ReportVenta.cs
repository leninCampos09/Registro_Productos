using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Registro_Productos.Models
{
    public class ReportVenta
    {
        [Key]
        public int Id { get; set; }

        public DateTime FechaGeneracion { get; set; }

        public DateTime? PeriodoInicio { get; set; }

        public DateTime? PeriodoFin { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalVentas { get; set; }

        public int TotalItems { get; set; }

        // JSON u otro detalle serializado para facilitar re-creación de informe
        public string? ReportData { get; set; }
    }
}
