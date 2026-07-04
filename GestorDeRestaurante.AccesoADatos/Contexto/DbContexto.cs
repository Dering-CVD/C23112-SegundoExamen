using GestorDeRestaurante.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace GestorDeRestaurante.AccesoADatos.Contexto;

public class DbContexto : DbContext
{
    public DbContexto(DbContextOptions<DbContexto> opciones) : base(opciones)
    {
    }

    public DbSet<Platillo> Platillos { get; set; }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Pedido> Pedido { get; set; }
    public DbSet<PedidoDetalle> PedidoDetalle { get; set; }
    public DbSet<Receta> Receta { get; set; }
    public DbSet<Ingrediente> Ingredientes { get; set; }
    public DbSet<RecetaDetalleIngrediente> RecetaDetalleIngredientes { get; set; }
    public DbSet<Factura> Facturas { get; set; }
    public DbSet<FacturaDetalle> FacturaDetalle { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Ingrediente>()
        .Property(i => i.UnidadDeMedida)
        .HasConversion<int>();

        modelBuilder.Entity<Usuario>()
        .Property(u => u.Rol)
        .HasConversion<string>();

        modelBuilder.Entity<Usuario>().HasData(new Usuario
        {
            Id = 1,
            Identificacion = "",
            Nombre = "",
            Apellidos = "",
            NombreUsuario = "Administrador",
            Email = "proyectorestaurante64@gmail.com",
            Contrasena = "Nuevo123*",
            Rol = Rol.Administrador
        }
        );
    }
}
