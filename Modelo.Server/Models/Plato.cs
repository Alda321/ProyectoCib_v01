using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Modelo.Server.Models
{
    public class Plato
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("precio")]
        public decimal Precio { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;

        // Relación con PedidoDetalle
        [JsonPropertyName("pedido_detalles")]
        public List<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
    }
}
