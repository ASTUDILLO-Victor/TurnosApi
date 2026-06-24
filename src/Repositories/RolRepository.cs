using TurnosApi.Data;
using TurnosApi.Models.Entities;
using TurnosApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TurnosApi.Repositories;

public class RolRepository : IRolRepository
{
    private readonly AppDbContext _context;

    public RolRepository(AppDbContext context)
    {
        _context = context;
    }

    public Rol? ObtenerPorNombre(string nombre)
    {
        return _context.Roles
            .Include(r => r.RolPermisos)
                .ThenInclude(rp => rp.Permiso)
            .FirstOrDefault(r => r.Nombre == nombre);
    }

    public List<Rol> ObtenerTodos()
    {
        return _context.Roles
            .Include(r => r.RolPermisos)
                .ThenInclude(rp => rp.Permiso)
            .ToList();
    }
}