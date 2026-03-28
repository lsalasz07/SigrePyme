using SigrePyme.Models;
using System.ComponentModel.DataAnnotations;

namespace SigrePyme.Models
{
    public class ProductoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código SKU es obligatorio.")]
        [StringLength(50, ErrorMessage = "El SKU no puede superar 50 caracteres.")]
        [Display(Name = "Código SKU")]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar 150 caracteres.")]
        [Display(Name = "Nombre del Producto")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio de costo es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de costo debe ser mayor a 0.")]
        [Display(Name = "Precio de Costo (₡)")]
        public decimal PrecioCosto { get; set; }

        [Required(ErrorMessage = "El precio de venta es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de venta debe ser mayor a 0.")]
        [Display(Name = "Precio de Venta (₡)")]
        public decimal PrecioVenta { get; set; }

        [Required(ErrorMessage = "El stock inicial es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        [Display(Name = "Stock Inicial")]
        public int StockActual { get; set; }

        [Required(ErrorMessage = "El umbral mínimo es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El umbral debe ser al menos 1.")]
        [Display(Name = "Stock Mínimo (Alerta)")]
        public int StockMinimo { get; set; } = 5;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }

    public class TransaccionViewModel
    {
        public int ProductoId { get; set; }

        [Display(Name = "Producto")]
        public string NombreProducto { get; set; } = string.Empty;

        [Display(Name = "Stock Actual")]
        public int StockActual { get; set; }

        [Required(ErrorMessage = "El tipo de transacción es obligatorio.")]
        [Display(Name = "Tipo de Movimiento")]
        public TipoTransaccion Tipo { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [StringLength(300)]
        [Display(Name = "Observación")]
        public string? Observacion { get; set; }
    }
}