using SigrePyme.Models;

namespace SigrePyme.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> ObtenerTodosAsync();
        Task<Cliente?> ObtenerPorIdAsync(int id);
        Task<(bool Exito, string Mensaje)> CrearAsync(ClienteViewModel vm);
        Task<(bool Exito, string Mensaje)> EditarAsync(ClienteViewModel vm);
        Task<(bool Exito, string Mensaje)> EliminarAsync(int id);
    }
}