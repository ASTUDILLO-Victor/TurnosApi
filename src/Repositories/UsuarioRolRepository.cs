using TurnosApi.Data;
using TurnosApi.Models.Entities;
using TurnosApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TurnosApi.Repositories;

public class UsuarioRolRepository : IUsuarioRolRepository
{
    private readonly AppDbContext _context;

    public UsuarioRolRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Agregar(UsuarioRol usuarioRol)
    {
        _context.UsuarioRoles.Add(usuarioRol);
        _context.SaveChanges();
    }

    public List<UsuarioRol> ObtenerPorUsuario(int usuarioId)
    {
        return _context.UsuarioRoles
            .Include(ur => ur.Rol)
                .ThenInclude(r => r!.RolPermisos)
                    .ThenInclude(rp => rp.Permiso)
            .Where(ur => ur.UsuarioId == usuarioId)
            .ToList();
    }
}