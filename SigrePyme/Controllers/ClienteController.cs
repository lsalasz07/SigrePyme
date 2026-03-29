using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SigrePyme.Constants;
using SigrePyme.Models;
using SigrePyme.Services;

namespace SIGRE_PYME.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var clientes = await _clienteService.ObtenerTodosAsync();
            return View(clientes);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id);
            if (cliente == null)
            {
                TempData["Error"] = "Cliente no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Vendedor}")]
        public IActionResult Create()
        {
            return View(new ClienteViewModel());
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Vendedor}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var (exito, mensaje) = await _clienteService.CrearAsync(vm);

            if (!exito)
            {
                ModelState.AddModelError(string.Empty, mensaje);
                return View(vm);
            }

            TempData["Exito"] = mensaje;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Vendedor}")]
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id);
            if (cliente == null)
            {
                TempData["Error"] = "Cliente no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new ClienteViewModel
            {
                Id = cliente.Id,
                NombreCompleto = cliente.NombreCompleto,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                Direccion = cliente.Direccion,
                Empresa = cliente.Empresa,
                Activo = cliente.Activo
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Vendedor}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClienteViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var (exito, mensaje) = await _clienteService.EditarAsync(vm);

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
            var cliente = await _clienteService.ObtenerPorIdAsync(id);
            if (cliente == null)
            {
                TempData["Error"] = "Cliente no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = Roles.Administrador)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (exito, mensaje) = await _clienteService.EliminarAsync(id);

            TempData[exito ? "Exito" : "Error"] = mensaje;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> BuscarJson(string q)
        {
            var todos = await _clienteService.ObtenerTodosAsync();

            var resultado = todos
                .Where(c => c.Activo && (
                    string.IsNullOrEmpty(q) ||
                    c.NombreCompleto.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    c.Email.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    (c.Empresa ?? "").Contains(q, StringComparison.OrdinalIgnoreCase)))
                .Take(10)
                .Select(c => new { c.Id, c.NombreCompleto, c.Email, c.Telefono, c.Empresa });

            return Json(resultado);
        }
    }
}