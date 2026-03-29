using Microsoft.EntityFrameworkCore;
using SigrePyme.Data;
using SigrePyme.Models;
using SigrePyme.Repositories;

namespace SigrePyme.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cliente>> ObtenerTodosAsync()
        {
            return await _context.Clientes
                .OrderBy(c => c.NombreCompleto)
                .ToListAsync();
        }

        public async Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cliente?> ObtenerPorEmailAsync(string email)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task AgregarAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
        }

        public void Actualizar(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
        }

        public void Eliminar(Cliente cliente)
        {
            cliente.Activo = false;
            _context.Clientes.Update(cliente);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}