using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GestorDeRestaurante.Dominio.Dtos;

public record ListarUsuarioDto(int Id, string Identificacion, string Nombre, string Apellidos, string Email, string Rol);

public class CrearUsuarioDto
{
    [Required(ErrorMessage = "La identificación es obligatoria")]
    [StringLength(10, ErrorMessage = "Máximo 10 caracteres")]
    public string Identificacion { get; set; } = null!;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre debe contener solo letras y espacios")]
    [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Los apellidos deben contener solo letras y espacios")]
    [StringLength(80, ErrorMessage = "Máximo 80 caracteres")]
    public string Apellidos { get; set; } = null!;

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    [RegularExpression(@"^\S+$", ErrorMessage = "El nombre de usuario no puede contener espacios")]
    public string NombreUsuario { get; set; } = null!;

    [Required(ErrorMessage = "El rol es obligatorio")]
    public string Rol { get; set; } = null!;
}

public class EditarUsuarioDto
{
    [Required(ErrorMessage = "La identificación es obligatoria")]
    public string Identificacion { get; set; } = null!;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre debe contener solo letras y espacios")]
    [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre debe contener solo letras y espacios")]
    [StringLength(80, ErrorMessage = "Máximo 80 caracteres")]
    public string Apellidos { get; set; } = null!;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "El rol es obligatorio")]
    public string Rol { get; set; } = null!;
}