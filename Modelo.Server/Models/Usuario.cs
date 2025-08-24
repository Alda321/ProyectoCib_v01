using System.ComponentModel.DataAnnotations;

namespace Modelo.Server.Models
{
    public enum RolUsuario
    {
        Mozo,
        Cocina,
        Admin
    }

    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string UsuarioLogin { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public RolUsuario Rol { get; set; }
    }
}
