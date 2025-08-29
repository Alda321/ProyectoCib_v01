using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Modelo.Server.Models
{
    public class PedidoDetalle
    {
        public int Id { get; set; }

        // Relación con Pedido
        public int PedidoId { get; set; }
        [JsonIgnore]
        public Pedido Pedido { get; set; }

        // Relación con Plato
        public int PlatoId { get; set; }
        [JsonIgnore]
        public Plato Plato { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }
}
