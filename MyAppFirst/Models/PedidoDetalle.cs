using System.ComponentModel.DataAnnotations;

namespace MyAppFirst.Models
{
    public class PedidoDetalle
    {
        public int Id { get; set; }

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
