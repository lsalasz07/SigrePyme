using SigrePyme.Models;

namespace SigrePyme.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task<(bool Exito, string Mensaje)> CrearAsync(ProductoViewModel vm);
        Task<(bool Exito, string Mensaje)> EditarAsync(ProductoViewModel vm);
        Task<(bool Exito, string Mensaje)> EliminarAsync(int id);
        Task<IEnumerable<Producto>> ObtenerConStockBajoAsync();
        Task<IEnumerable<Producto>> BuscarAsync(string q);

        Task<IEnumerable<TransaccionInventario>> ObtenerHistorialAsync(int productoId);
        Task<(bool Exito, string Mensaje)> RegistrarTransaccionAsync(
            TransaccionViewModel vm, string usuarioEmail);
    }
}
