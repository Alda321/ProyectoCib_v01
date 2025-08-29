using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Modelo.Server.Models
{
    public enum EstadoPedido
    {
        Pendiente,      // El mesero lo envió, pero aún no se prepara
        Preparando,     // Cocina lo está preparando
        Listo,          // Pedido terminado en cocina
        Cancelado,      // Cancelado por cualquier razón
        Servido         // Entregado a la mesa
    }

    public class Pedido
    {
        public int Id { get; set; }

        [Required]
        public string NumeroMesa { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;

        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;

        public bool Pagado { get; set; } = false;
        public string? Comentario { get; set; }
        [JsonIgnore]

        // Relación con Detalles del Pedido
        public ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();


        // Relación con el mozo que atendió
        public int? MozoId { get; set; }
        [JsonIgnore]
        public Usuario? Mozo { get; set; }
    }
}
