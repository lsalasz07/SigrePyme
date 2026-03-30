using SigrePyme.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class MovimientoInventario
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; }

    public int Cantidad { get; set; }
    public string TipoMovimiento { get; set; } // "ENTRADA" o "SALIDA"
    public DateTime Fecha { get; set; }
}