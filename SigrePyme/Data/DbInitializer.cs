using Microsoft.AspNetCore.Identity;
using SigrePyme.Models;

namespace SigrePyme.Helpers
{
    public static class DbInitializer
    {
        public static async Task InicializarAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Administrador", "Gerente", "Vendedor", "Almacenista" };

            foreach (var rol in roles)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                    await roleManager.CreateAsync(new IdentityRole(rol));
            }

            const string adminEmail = "admin@gmail.com";
            const string adminPassword = "Admin01*";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NombreCompleto = "Administrador del Sistema",
                    FechaRegistro = DateTime.Now,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Administrador");
            }
        }
    }
}