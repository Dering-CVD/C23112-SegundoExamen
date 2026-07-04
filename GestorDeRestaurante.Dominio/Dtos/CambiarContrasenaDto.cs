using System.ComponentModel.DataAnnotations;

namespace GestorDeRestaurante.Dominio.Dtos;

public class CambiarContrasenaDto
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    public string NombreUsuario { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña actual es obligatoria")]
    public string ContrasenaActual { get; set; } = null!;

    [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "La clave debe tener al menos 8 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "La clave debe incluir mayúscula, minúscula, número, carácter especial y tener una longitud de al menos 8 caracteres")]
    public string ContrasenaNueva { get; set; } = null!;

    [Required(ErrorMessage = "Debe confirmar la nueva contraseña")]
    [Compare("ContrasenaNueva", ErrorMessage = "La nueva contraseña no coincide")]
    public string ConfirmarContrasena { get; set; } = null!;
}
