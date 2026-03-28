using SigrePyme.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SigrePyme.Models
{
    public enum TipoTransaccion
    {
        [Display(Name = "Entrada (Compra)")]
        Entrada = 1,

        [Display(Name = "Salida (Venta)")]
        Salida = 2,

        [Display(Name = "Ajuste / Merma")]
        Ajuste = 3
    }

    public class TransaccionInventario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Producto")]
        public int ProductoId { get; set; }

        [ForeignKey(nameof(ProductoId))]
        public Producto? Producto { get; set; }

        [Required(ErrorMessage = "El tipo de transacción es obligatorio.")]
        [Display(Name = "Tipo")]
        public TipoTransaccion Tipo { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [StringLength(300)]
        [Display(Name = "Observación")]
        public string? Observacion { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [StringLength(100)]
        [Display(Name = "Registrado por")]
        public string UsuarioRegistro { get; set; } = string.Empty;

        [Display(Name = "Stock resultante")]
        public int StockResultante { get; set; }
    }
}