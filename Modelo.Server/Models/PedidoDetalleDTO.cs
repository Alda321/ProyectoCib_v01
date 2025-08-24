using System.ComponentModel.DataAnnotations;

namespace Modelo.Server.Models
{
    public class PedidoDetalleDTO
    {

        // Relación con Pedido
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }

        // Relación con Plato
        public int PlatoId { get; set; }
        public Plato Plato { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }
}
