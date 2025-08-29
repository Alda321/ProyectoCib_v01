using System.ComponentModel.DataAnnotations;

namespace Modelo.Server.Models
{
   public class PedidoDetalleDTO
    {
        [Required]
        public int PedidoId { get; set; }

        [Required]
        public int PlatoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }
}
