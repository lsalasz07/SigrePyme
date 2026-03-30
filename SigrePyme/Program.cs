using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SigrePyme.Data;
using SigrePyme.Helpers;
using SigrePyme.Models;
using SigrePyme.Repositories;
using SigrePyme.Services;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// BASE DE DATOS
// =============================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 0))
    ));

// =============================================
// ASP.NET CORE IDENTITY
// =============================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccesoDenegado";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

// =============================================
// REPOSITORIOS
// =============================================
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ITransaccionRepository, TransaccionRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

// =============================================
// SERVICIOS
// =============================================
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();

// =============================================
// MVC
// =============================================
builder.Services.AddControllersWithViews();

var app = builder.Build();

// =============================================
// SEED
// =============================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

   // await db.Database.MigrateAsync();
    //await DbInitializer.InicializarAsync(userManager, roleManager);
}

// =============================================
// PIPELINE HTTP
// =============================================
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

