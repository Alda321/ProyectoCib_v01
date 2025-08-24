using Microsoft.EntityFrameworkCore;
using Modelo.Server.Models;

namespace Modelo.Server.Data
{
    public class MyAppContext : DbContext 
    {
        public MyAppContext(DbContextOptions<MyAppContext> options)
            : base(options)
        {
        }

        public DbSet<Plato> Platos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalle> PedidoDetalles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación Pedido -> PedidoDetalle
            modelBuilder.Entity<PedidoDetalle>()
                .HasOne(pd => pd.Pedido)
                .WithMany(p => p.Detalles)
                .HasForeignKey(pd => pd.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Plato -> PedidoDetalle
            modelBuilder.Entity<PedidoDetalle>()
                .HasOne(pd => pd.Plato)
                .WithMany(p => p.PedidoDetalles)
                .HasForeignKey(pd => pd.PlatoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
