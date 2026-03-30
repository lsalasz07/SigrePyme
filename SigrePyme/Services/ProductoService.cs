using SigrePyme.Models;
using SigrePyme.Repositories;
using SigrePyme.Services;

namespace SigrePyme.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepo;
        private readonly ITransaccionRepository _transaccionRepo;

        public ProductoService(
            IProductoRepository productoRepo,
            ITransaccionRepository transaccionRepo)
        {
            _productoRepo = productoRepo;
            _transaccionRepo = transaccionRepo;
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
            => await _productoRepo.ObtenerTodosAsync();

        public async Task<Producto?> ObtenerPorIdAsync(int id)
            => await _productoRepo.ObtenerPorIdAsync(id);

        public async Task<IEnumerable<Producto>> ObtenerConStockBajoAsync()
            => await _productoRepo.ObtenerConStockBajoAsync();

        public async Task<(bool Exito, string Mensaje)> CrearAsync(ProductoViewModel vm)
        {
            var existente = await _productoRepo.ObtenerPorSkuAsync(vm.SKU.Trim());
            if (existente != null)
                return (false, $"Ya existe un producto con el SKU '{vm.SKU}'. Use uno diferente.");

            var producto = new Producto
            {
                SKU = vm.SKU.Trim().ToUpper(),
                Nombre = vm.Nombre.Trim(),
                Descripcion = vm.Descripcion?.Trim(),
                PrecioCosto = vm.PrecioCosto,
                PrecioVenta = vm.PrecioVenta,
                StockActual = vm.StockActual,
                StockMinimo = vm.StockMinimo,
                Activo = true,
                FechaRegistro = DateTime.Now
            };

            await _productoRepo.AgregarAsync(producto);
            await _productoRepo.GuardarCambiosAsync();

            if (vm.StockActual > 0)
            {
                var transaccion = new TransaccionInventario
                {
                    ProductoId = producto.Id,
                    Tipo = TipoTransaccion.Entrada,
                    Cantidad = vm.StockActual,
                    Observacion = "Stock inicial al crear el producto.",
                    Fecha = DateTime.Now,
                    UsuarioRegistro = "Sistema",
                    StockResultante = vm.StockActual
                };
                await _transaccionRepo.AgregarAsync(transaccion);
                await _transaccionRepo.GuardarCambiosAsync();
            }

            return (true, $"Producto '{producto.Nombre}' creado correctamente.");
        }

        public async Task<(bool Exito, string Mensaje)> EditarAsync(ProductoViewModel vm)
        {
            var producto = await _productoRepo.ObtenerPorIdAsync(vm.Id);
            if (producto == null)
                return (false, "Producto no encontrado.");

            var otroCon = await _productoRepo.ObtenerPorSkuAsync(vm.SKU.Trim());
            if (otroCon != null && otroCon.Id != vm.Id)
                return (false, $"El SKU '{vm.SKU}' ya está en uso por otro producto.");

            producto.SKU = vm.SKU.Trim().ToUpper();
            producto.Nombre = vm.Nombre.Trim();
            producto.Descripcion = vm.Descripcion?.Trim();
            producto.PrecioCosto = vm.PrecioCosto;
            producto.PrecioVenta = vm.PrecioVenta;
            producto.StockMinimo = vm.StockMinimo;
            producto.Activo = vm.Activo;

            _productoRepo.Actualizar(producto);
            await _productoRepo.GuardarCambiosAsync();

            return (true, $"Producto '{producto.Nombre}' actualizado correctamente.");
        }

        public async Task<(bool Exito, string Mensaje)> EliminarAsync(int id)
        {
            var producto = await _productoRepo.ObtenerPorIdAsync(id);
            if (producto == null)
                return (false, "Producto no encontrado.");

            if (producto.Transacciones.Any())
                return (false,
                    $"No se puede eliminar '{producto.Nombre}' porque tiene " +
                    $"{producto.Transacciones.Count} transacción(es) registrada(s). " +
                    "Use la opción 'Desactivar' en su lugar.");

            _productoRepo.Eliminar(producto);
            await _productoRepo.GuardarCambiosAsync();

            return (true, $"Producto '{producto.Nombre}' desactivado correctamente.");
        }

        public async Task<IEnumerable<TransaccionInventario>> ObtenerHistorialAsync(int productoId)
            => await _transaccionRepo.ObtenerPorProductoAsync(productoId);

        public async Task<(bool Exito, string Mensaje)> RegistrarTransaccionAsync(
            TransaccionViewModel vm, string usuarioEmail)
        {
            var producto = await _productoRepo.ObtenerPorIdAsync(vm.ProductoId);
            if (producto == null)
                return (false, "Producto no encontrado.");

            if (vm.Tipo == TipoTransaccion.Salida || vm.Tipo == TipoTransaccion.Ajuste)
            {
                if (vm.Cantidad > producto.StockActual)
                    return (false,
                        $"Stock insuficiente. Stock actual: {producto.StockActual} unidades. " +
                        $"Cantidad solicitada: {vm.Cantidad}.");
            }

            if (vm.Tipo == TipoTransaccion.Entrada)
                producto.StockActual += vm.Cantidad;
            else
                producto.StockActual -= vm.Cantidad;

            int stockResultante = producto.StockActual;

            _productoRepo.Actualizar(producto);

            var transaccion = new TransaccionInventario
            {
                ProductoId = vm.ProductoId,
                Tipo = vm.Tipo,
                Cantidad = vm.Cantidad,
                Observacion = vm.Observacion?.Trim(),
                Fecha = DateTime.Now,
                UsuarioRegistro = usuarioEmail,
                StockResultante = stockResultante
            };

            await _transaccionRepo.AgregarAsync(transaccion);
            await _transaccionRepo.GuardarCambiosAsync();

            string alerta = producto.TieneStockBajo
                ? $" ⚠️ Atención: el stock actual ({producto.StockActual}) está por debajo del mínimo ({producto.StockMinimo})."
                : string.Empty;

            return (true, $"Transacción registrada correctamente. Stock actual: {producto.StockActual}.{alerta}");
        }
        public async Task<IEnumerable<Producto>> BuscarAsync(string q)
        {
            return await _productoRepo.BuscarAsync(q);
        }

    }
}