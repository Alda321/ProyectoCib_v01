using Microsoft.EntityFrameworkCore;
using MyAppFirst.Models;

namespace MyAppFirst.Data
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions<MyAppContext> options) : base(options) { }

        public DbSet<Plato> Platos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalle> PedidoDetalles { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación Pedido → PedidoDetalle (1 a muchos)
            modelBuilder.Entity<Pedido>()
                .HasMany(p => p.Detalles)
                .WithOne(d => d.Pedido)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Plato → PedidoDetalle (1 a muchos)
            modelBuilder.Entity<Plato>()
                .HasMany(p => p.PedidoDetalles)
                .WithOne(d => d.Plato)
                .HasForeignKey(d => d.PlatoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
