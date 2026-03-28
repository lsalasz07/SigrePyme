using SigrePyme.Models;

namespace SigrePyme.Repositories
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task<Producto?> ObtenerPorSkuAsync(string sku);
        Task<IEnumerable<Producto>> ObtenerConStockBajoAsync();
        Task AgregarAsync(Producto producto);
        void Actualizar(Producto producto);
        void Eliminar(Producto producto);
        Task GuardarCambiosAsync();
    }
}
