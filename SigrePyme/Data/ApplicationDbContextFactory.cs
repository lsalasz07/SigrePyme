using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SigrePyme.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuracion = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var cadenaConexion = configuracion.GetConnectionString("ConexionPredeterminada");

            optionsBuilder.UseMySql(
                cadenaConexion,
                ServerVersion.AutoDetect(cadenaConexion)
            );

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}