using SigrePyme.Models;

namespace SigrePyme.Repositories
{
    public interface ITransaccionRepository
    {
        Task<IEnumerable<TransaccionInventario>> ObtenerPorProductoAsync(int productoId);
        Task AgregarAsync(TransaccionInventario transaccion);
        Task GuardarCambiosAsync();
    }
}