namespace Modelo.Server.Models
{
    public class PlatoDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public bool Activo { get; set; } = true;

        // Relación con PedidoDetalle
        public List<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
    }
}
