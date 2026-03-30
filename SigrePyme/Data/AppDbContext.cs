using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SigrePyme.Models;

namespace SigrePyme.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }


        public DbSet<Producto> Productos { get; set; }
        public DbSet<TransaccionInventario> TransaccionesInventario { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Producto>(entity =>
            {
                entity.HasIndex(p => p.SKU).IsUnique();
                entity.Property(p => p.PrecioCosto).HasColumnType("decimal(18,2)");
                entity.Property(p => p.PrecioVenta).HasColumnType("decimal(18,2)");
            });

            builder.Entity<TransaccionInventario>(entity =>
            {
                entity.HasOne(t => t.Producto)
                      .WithMany(p => p.Transacciones)
                      .HasForeignKey(t => t.ProductoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Cliente>(entity =>
            {
                entity.HasIndex(c => c.Email).IsUnique();
            });
        }
    }
}