using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SigrePyme.Data;
using SigrePyme.Models;

var builder = WebApplication.CreateBuilder(args);

var cadenaConexion = builder.Configuration.GetConnectionString("ConexionPredeterminada")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'ConexionPredeterminada'.");

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseMySql(cadenaConexion, ServerVersion.AutoDetect(cadenaConexion))
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opciones =>
{
    opciones.Password.RequiredLength = 6;
    opciones.Password.RequireDigit = true;
    opciones.Password.RequireUppercase = true;
    opciones.Password.RequireLowercase = true;
    opciones.Password.RequireNonAlphanumeric = false;

    opciones.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    opciones.Lockout.MaxFailedAccessAttempts = 5;
    opciones.Lockout.AllowedForNewUsers = true;

    opciones.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opciones =>
{
    opciones.LoginPath = "/Cuenta/Login";
    opciones.LogoutPath = "/Cuenta/CerrarSesion";
    opciones.AccessDeniedPath = "/Cuenta/AccesoDenegado";
    opciones.ExpireTimeSpan = TimeSpan.FromHours(8);
    opciones.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Administrador", "Vendedor", "Almacenista", "Gerente" };

    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol))
        {
            await roleManager.CreateAsync(new IdentityRole(rol));
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();