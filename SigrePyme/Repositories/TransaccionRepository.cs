using Microsoft.EntityFrameworkCore;
using SigrePyme.Data;
using SigrePyme.Models;

namespace SigrePyme.Repositories
{
    public class TransaccionRepository : ITransaccionRepository
    {
        private readonly AppDbContext _context;

        public TransaccionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransaccionInventario>> ObtenerPorProductoAsync(int productoId)
        {
            return await _context.TransaccionesInventario
                .Where(t => t.ProductoId == productoId)
                .OrderByDescending(t => t.Fecha)
                .ToListAsync();
        }

        public async Task AgregarAsync(TransaccionInventario transaccion)
        {
            await _context.TransaccionesInventario.AddAsync(transaccion);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}