using SistemaAlquilerPlaya.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace GestorDeRestaurante.AccesoADatos.Contexto;

public class DbContexto : DbContext
{
    public DbContexto(DbContextOptions<DbContexto> opciones)
        : base(opciones)
    {
    }

    public DbSet<Articulo> Articulos { get; set; }

    public DbSet<Alquiler> Alquileres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Alquiler>()
            .HasOne(a => a.Articulo)
            .WithMany()
            .HasForeignKey(a => a.ArticuloId);

        modelBuilder.Entity<Articulo>()
            .Property(a => a.PrecioHora)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Alquiler>()
            .Property(a => a.MontoTotal)
            .HasColumnType("decimal(18,2)");
    }
}