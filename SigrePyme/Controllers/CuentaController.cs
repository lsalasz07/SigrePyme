using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SigrePyme.Models;
using SigrePyme.ViewModels;

namespace SigrePyme.Controllers
{
    public class CuentaController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CuentaController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Registro()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var usuario = new ApplicationUser
            {
                UserName = modelo.Correo,
                Email = modelo.Correo,
                NombreCompleto = modelo.NombreCompleto,
                FechaCreacion = DateTime.Now
            };

            var resultado = await _userManager.CreateAsync(usuario, modelo.Contrasena);

            if (resultado.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(modelo.Rol))
                {
                    ModelState.AddModelError(string.Empty, "El rol seleccionado no existe.");
                    return View(modelo);
                }

                await _userManager.AddToRoleAsync(usuario, modelo.Rol);

                await _signInManager.SignInAsync(usuario, isPersistent: false);

                TempData["Exito"] = $"¡Bienvenido/a {usuario.NombreCompleto}! Su cuenta ha sido creada exitosamente.";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, TraducirErrorIdentity(error.Code));
            }

            return View(modelo);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(modelo);

            var resultado = await _signInManager.PasswordSignInAsync(
                modelo.Correo,
                modelo.Contrasena,
                modelo.Recordarme,
                lockoutOnFailure: true
            );

            if (resultado.Succeeded)
            {
                TempData["Exito"] = "Sesión iniciada correctamente.";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            if (resultado.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty,
                    "Su cuenta ha sido bloqueada por múltiples intentos fallidos. Intente de nuevo más tarde.");
            }
            else
            {
                ModelState.AddModelError(string.Empty,
                    "Correo electrónico o contraseña incorrectos.");
            }

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CerrarSesion()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Cuenta");
        }

        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }

        private static string TraducirErrorIdentity(string codigo)
        {
            return codigo switch
            {
                "PasswordTooShort" => "La contraseña debe tener al menos 6 caracteres.",
                "PasswordRequiresUpper" => "La contraseña debe contener al menos una letra mayúscula.",
                "PasswordRequiresLower" => "La contraseña debe contener al menos una letra minúscula.",
                "PasswordRequiresDigit" => "La contraseña debe contener al menos un número.",
                "PasswordRequiresNonAlphanumeric" => "La contraseña debe contener al menos un carácter especial.",
                "DuplicateUserName" => "Ya existe una cuenta registrada con ese correo electrónico.",
                "DuplicateEmail" => "Ya existe una cuenta registrada con ese correo electrónico.",
                _ => "Ocurrió un error al procesar su solicitud."
            };
        }
    }
}