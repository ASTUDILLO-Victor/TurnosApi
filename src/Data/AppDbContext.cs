using Microsoft.EntityFrameworkCore;
using TurnosApi.Models.Entities;

namespace TurnosApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<TokenBlacklist> TokenBlacklist { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Permiso> Permisos { get; set; }
    public DbSet<UsuarioRol> UsuarioRoles { get; set; }
    public DbSet<RolPermiso> RolPermisos { get; set; }
    public DbSet<Turno> Turnos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Clave compuesta para UsuarioRol
        modelBuilder.Entity<UsuarioRol>()
            .HasKey(ur => new { ur.UsuarioId, ur.RolId });

        // Clave compuesta para RolPermiso
        modelBuilder.Entity<RolPermiso>()
            .HasKey(rp => new { rp.RolId, rp.PermisoId });

        // Dos foreign keys de Turno a Usuario
        modelBuilder.Entity<Turno>()
            .HasOne(t => t.Medico)
            .WithMany()
            .HasForeignKey(t => t.MedicoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Turno>()
            .HasOne(t => t.Paciente)
            .WithMany()
            .HasForeignKey(t => t.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}