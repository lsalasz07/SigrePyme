using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SigrePyme.Constants;
using SigrePyme.Models;
using SigrePyme.Services;

namespace SIGRE_PYME.Controllers
{

    [Authorize]
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var productos = await _productoService.ObtenerTodosAsync();

            var alertas = await _productoService.ObtenerConStockBajoAsync();
            ViewBag.CantidadAlertas = alertas.Count();

            return View(productos);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Almacenista}")]
        public IActionResult Create()
        {
            return View(new ProductoViewModel());
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Almacenista}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var (exito, mensaje) = await _productoService.CrearAsync(vm);

            if (!exito)
            {
                ModelState.AddModelError(string.Empty, mensaje);
                return View(vm);
            }

            TempData["Exito"] = mensaje;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Almacenista}")]
        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new ProductoViewModel
            {
                Id = producto.Id,
                SKU = producto.SKU,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                PrecioCosto = producto.PrecioCosto,
                PrecioVenta = producto.PrecioVenta,
                StockActual = producto.StockActual,
                StockMinimo = producto.StockMinimo,
                Activo = producto.Activo
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Almacenista}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductoViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var (exito, mensaje) = await _productoService.EditarAsync(vm);

            if (!exito)
            {
                ModelState.AddModelError(string.Empty, mensaje);
                return View(vm);
            }

            TempData["Exito"] = mensaje;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = Roles.Administrador)]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = Roles.Administrador)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (exito, mensaje) = await _productoService.EliminarAsync(id);

            if (!exito)
                TempData["Error"] = mensaje;
            else
                TempData["Exito"] = mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Transacciones(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var historial = await _productoService.ObtenerHistorialAsync(id);

            ViewBag.Producto = producto;
            return View(historial);
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Almacenista}")]
        public async Task<IActionResult> RegistrarMovimiento(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new TransaccionViewModel
            {
                ProductoId = producto.Id,
                NombreProducto = producto.Nombre,
                StockActual = producto.StockActual
            };

            CargarTiposTransaccion();
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Almacenista}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarMovimiento(TransaccionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                CargarTiposTransaccion();
                return View(vm);
            }

            var usuarioEmail = User.Identity?.Name ?? "Desconocido";
            var (exito, mensaje) = await _productoService.RegistrarTransaccionAsync(vm, usuarioEmail);

            if (!exito)
            {
                ModelState.AddModelError(string.Empty, mensaje);
                CargarTiposTransaccion();
                return View(vm);
            }

            TempData["Exito"] = mensaje;
            return RedirectToAction(nameof(Transacciones), new { id = vm.ProductoId });
        }

        [HttpGet]
        public async Task<IActionResult> BuscarJson(string q)
        {
            var todos = await _productoService.ObtenerTodosAsync();

            var resultado = todos
                .Where(p => string.IsNullOrEmpty(q) ||
                            p.Nombre.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                            p.SKU.Contains(q, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .Select(p => new
                {
                    p.Id,
                    p.SKU,
                    p.Nombre,
                    p.PrecioVenta,
                    p.StockActual
                });

            return Json(resultado);
        }

        private void CargarTiposTransaccion()
        {
            ViewBag.Tipos = new List<SelectListItem>
            {
                new() { Value = ((int)TipoTransaccion.Entrada).ToString(), Text = "Entrada (Compra / Reposición)" },
                new() { Value = ((int)TipoTransaccion.Salida).ToString(),  Text = "Salida (Venta / Consumo)"     },
                new() { Value = ((int)TipoTransaccion.Ajuste).ToString(),  Text = "Ajuste / Merma"              }
            };
        }
    }
}