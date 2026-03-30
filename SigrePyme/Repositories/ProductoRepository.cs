using Microsoft.EntityFrameworkCore;
using SigrePyme.Data;
using SigrePyme.Models;
using SigrePyme.Repositories;

namespace SigrePyme.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly AppDbContext _context;

        public ProductoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
        {
            return await _context.Productos
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Transacciones)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Producto?> ObtenerPorSkuAsync(string sku)
        {
            return await _context.Productos
                .FirstOrDefaultAsync(p => p.SKU == sku);
        }

        public async Task<IEnumerable<Producto>> ObtenerConStockBajoAsync()
        {
            return await _context.Productos
                .Where(p => p.Activo && p.StockActual <= p.StockMinimo)
                .OrderBy(p => p.StockActual)
                .ToListAsync();
        }

        public async Task AgregarAsync(Producto producto)
        {
            await _context.Productos.AddAsync(producto);
        }

        public void Actualizar(Producto producto)
        {
            _context.Productos.Update(producto);
        }

        public void Eliminar(Producto producto)
        {
            producto.Activo = false;
            _context.Productos.Update(producto);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Producto>> BuscarAsync(string q)
        {
            var query = _context.Productos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(p =>
                    p.Nombre.Contains(q) ||
                    p.SKU.Contains(q));
            }

            return await query
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .Take(10)
                .ToListAsync();
        }

    }
}