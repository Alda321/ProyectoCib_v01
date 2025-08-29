namespace Modelo.Server.Models
{
    public class PedidoDTO
    {
        public int Id { get; set; }

        public string NumeroMesa { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        public string Estado { get; set; } = string.Empty; 

        public bool Pagado { get; set; }

        public string? Comentario { get; set; }

        // Lista de detalles (DTOs también)
        public List<PedidoDetalleDTO> Detalles { get; set; } = new List<PedidoDetalleDTO>();

        // Relación con el mozo (se puede exponer solo lo necesario)
        public int? MozoId { get; set; }
        public string? NombreMozo { get; set; }
    }
}
