using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigrePyme.Data;
using SigrePyme.Models;

namespace SigrePyme.Controllers
{
    [Authorize(Roles = "Administrador,Gerente")]
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("kpis")]
        public async Task<IActionResult> ObtenerKpis()
        {
            var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var totalProductos = await _context.Productos
                .Where(p => p.Activo)
                .CountAsync();

            var productosStockBajo = await _context.Productos
                .Where(p => p.Activo && p.StockActual <= p.StockMinimo)
                .Select(p => new
                {
                    p.SKU,
                    p.Nombre,
                    p.StockActual,
                    p.StockMinimo
                })
                .OrderBy(p => p.StockActual)
                .Take(5)
                .ToListAsync();

            var totalClientes = await _context.Clientes
                .Where(c => c.Activo)
                .CountAsync();

            var clientesNuevosEsteMes = await _context.Clientes
                .Where(c => c.Activo && c.FechaRegistro >= inicioMes)
                .CountAsync();

            var entradasEsteMes = await _context.TransaccionesInventario
                .Where(t => t.Tipo == TipoTransaccion.Entrada && t.Fecha >= inicioMes)
                .CountAsync();

            var salidasEsteMes = await _context.TransaccionesInventario
                .Where(t => t.Tipo == TipoTransaccion.Salida && t.Fecha >= inicioMes)
                .CountAsync();

            var productoMasMovimientos = await _context.TransaccionesInventario
                .GroupBy(t => t.Producto!.Nombre)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync() ?? "Sin movimientos";

            var movimientosRecientes = await _context.TransaccionesInventario
                .Include(t => t.Producto)
                .OrderByDescending(t => t.Fecha)
                .Take(8)
                .Select(t => new
                {
                    fecha = t.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    producto = t.Producto!.Nombre,
                    tipo = t.Tipo.ToString(),
                    cantidad = t.Cantidad,
                    stock = t.StockResultante
                })
                .ToListAsync();

            return Ok(new
            {
                totalProductos,
                productosStockBajo,
                cantidadStockBajo = productosStockBajo.Count,
                totalClientes,
                clientesNuevosEsteMes,
                entradasEsteMes,
                salidasEsteMes,
                productoMasMovimientos,
                movimientosRecientes
            });
        }
    }
}