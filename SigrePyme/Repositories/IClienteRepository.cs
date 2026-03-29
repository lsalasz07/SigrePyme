using SigrePyme.Models;

namespace SigrePyme.Repositories
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> ObtenerTodosAsync();
        Task<Cliente?> ObtenerPorIdAsync(int id);
        Task<Cliente?> ObtenerPorEmailAsync(string email);
        Task AgregarAsync(Cliente cliente);
        void Actualizar(Cliente cliente);
        void Eliminar(Cliente cliente);
        Task GuardarCambiosAsync();
    }
}