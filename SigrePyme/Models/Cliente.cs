using System.ComponentModel.DataAnnotations;

namespace SigrePyme.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(150)]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        [StringLength(150)]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(250)]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [StringLength(100)]
        [Display(Name = "Empresa / Razón Social")]
        public string? Empresa { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}