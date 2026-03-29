using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SigrePyme.Constants;
using SigrePyme.Models;

namespace SigrePyme.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewBag.HayUsuarios = _userManager.Users.Any();
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.HayUsuarios = _userManager.Users.Any();

            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToLocal(returnUrl);

            ModelState.AddModelError(string.Empty, "Correo o contrasena incorrectos. Intente de nuevo.");
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            bool esPrimerUsuario = !_userManager.Users.Any();
            ViewBag.EsPrimerUsuario = esPrimerUsuario;
            CargarRolesEnVista(esPrimerUsuario);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            bool esPrimerUsuario = !_userManager.Users.Any();
            ViewBag.EsPrimerUsuario = esPrimerUsuario;

            model.Rol = esPrimerUsuario ? Roles.Administrador : Roles.Cliente;

            if (!ModelState.IsValid)
            {
                CargarRolesEnVista(esPrimerUsuario);
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                NombreCompleto = model.NombreCompleto,
                FechaRegistro = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(model.Rol))
                    await _roleManager.CreateAsync(new IdentityRole(model.Rol));

                await _userManager.AddToRoleAsync(user, model.Rol);
                await _signInManager.SignInAsync(user, isPersistent: false);

                TempData["Exito"] = esPrimerUsuario
                    ? $"Bienvenido, {user.NombreCompleto}! Cuenta de Administrador creada."
                    : $"Bienvenido, {user.NombreCompleto}! Tu cuenta fue creada con rol Cliente.";

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            CargarRolesEnVista(esPrimerUsuario);
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = Roles.Administrador)]
        public IActionResult RegisterAdmin()
        {
            ViewBag.EsPrimerUsuario = false;
            CargarRolesEnVista(soloAdmin: false);
            return View("Register");
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrador)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(RegisterViewModel model)
        {
            ViewBag.EsPrimerUsuario = false;

            if (!ModelState.IsValid)
            {
                CargarRolesEnVista(soloAdmin: false);
                return View("Register", model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                NombreCompleto = model.NombreCompleto,
                FechaRegistro = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(model.Rol))
                    await _roleManager.CreateAsync(new IdentityRole(model.Rol));

                await _userManager.AddToRoleAsync(user, model.Rol);

                TempData["Exito"] = $"Usuario '{model.NombreCompleto}' registrado con el rol '{model.Rol}'.";
                return RedirectToAction(nameof(ListarUsuarios));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            CargarRolesEnVista(soloAdmin: false);
            return View("Register", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [Authorize(Roles = Roles.Administrador)]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarios = _userManager.Users.ToList();
            var lista = new List<(ApplicationUser Usuario, IList<string> Roles)>();

            foreach (var u in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(u);
                lista.Add((u, roles));
            }

            return View(lista);
        }

        [AllowAnonymous]
        public IActionResult AccesoDenegado() => View();

        private void CargarRolesEnVista(bool soloAdmin = false)
        {
            if (soloAdmin)
            {
                ViewBag.Roles = new List<SelectListItem>
                {
                    new() { Value = Roles.Administrador, Text = "Administrador", Selected = true }
                };
            }
            else
            {
                ViewBag.Roles = new List<SelectListItem>
                {
                    new() { Value = Roles.Administrador, Text = Roles.Administrador },
                    new() { Value = Roles.Gerente,       Text = Roles.Gerente       },
                    new() { Value = Roles.Vendedor,      Text = Roles.Vendedor      },
                    new() { Value = Roles.Almacenista,   Text = Roles.Almacenista   }
                };
            }
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }
    }
}