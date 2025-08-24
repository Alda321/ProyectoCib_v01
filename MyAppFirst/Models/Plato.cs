using System.ComponentModel.DataAnnotations;

namespace MyAppFirst.Models
{
    public class Plato
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        public decimal Precio { get; set; }


        public bool Activo { get; set; } = true; 

        // Relación con PedidoDetalle
        public List<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
    }
}
