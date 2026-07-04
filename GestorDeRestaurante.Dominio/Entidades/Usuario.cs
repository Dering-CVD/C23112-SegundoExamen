namespace GestorDeRestaurante.Dominio.Entidades;

public class Usuario
{
    public int Id { get; set; }
    public string Identificacion { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Contrasena { get; set; } = null!;
    public Rol Rol { get; set; }
    public int IntentosFallidos { get; set; } = 0;
    public DateTime? BloqueadoHasta { get; set; }
}