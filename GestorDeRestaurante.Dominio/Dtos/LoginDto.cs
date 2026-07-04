using System.ComponentModel.DataAnnotations;

namespace GestorDeRestaurante.Dominio.Dtos;

public record LoginDto(
    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    string NombreUsuario,

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    string Contrasena
);