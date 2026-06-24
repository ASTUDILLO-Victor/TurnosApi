using Microsoft.EntityFrameworkCore;
using TurnosApi.Data;
using TurnosApi.Models.Entities;
using TurnosApi.Repositories.Interfaces;

namespace TurnosApi.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public Usuario? ObtenerPorId(int id)
    {
        return _context.Usuarios
            .Include(u => u.RefreshTokens)
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
            .FirstOrDefault(u => u.Id == id);
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        return _context.Usuarios
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
            .FirstOrDefault(u => u.Email == email);
    }

    public bool ExisteEmail(string email)
    {
        return _context.Usuarios.Any(u => u.Email == email);
    }

    public Usuario Agregar(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
        return usuario;
    }

    public Usuario Actualizar(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        _context.SaveChanges();
        return usuario;
    }

    public List<Usuario> ObtenerTodos()
    {
        return _context.Usuarios.ToList();
    }
    
}