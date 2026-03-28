using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SigrePyme.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El código SKU es obligatorio.")]
        [StringLength(50)]
        [Display(Name = "Código SKU")]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(150)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio de costo es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0.")]
        [Display(Name = "Precio de Costo")]
        public decimal PrecioCosto { get; set; }

        [Required(ErrorMessage = "El precio de venta es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0.")]
        [Display(Name = "Precio de Venta")]
        public decimal PrecioVenta { get; set; }

        [Required]
        [Display(Name = "Stock Actual")]
        public int StockActual { get; set; } = 0;

        [Required(ErrorMessage = "El umbral mínimo es obligatorio.")]
        [Display(Name = "Stock Mínimo (Umbral de Alerta)")]
        public int StockMinimo { get; set; } = 5;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public ICollection<TransaccionInventario> Transacciones { get; set; }
            = new List<TransaccionInventario>();

        [NotMapped]
        public bool TieneStockBajo => StockActual <= StockMinimo;
    }
}