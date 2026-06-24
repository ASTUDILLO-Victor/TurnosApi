using TurnosApi.Data;
using TurnosApi.Models.Entities;
using TurnosApi.Repositories.Interfaces;

namespace TurnosApi.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public RefreshToken Agregar(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        _context.SaveChanges();
        return refreshToken;
    }

    public RefreshToken? ObtenerPorToken(string token)
    {
        return _context.RefreshTokens
            .FirstOrDefault(rt => rt.Token == token);
    }

    public void Revocar(RefreshToken refreshToken)
    {
        refreshToken.EstaRevocado = true;
        _context.SaveChanges();
    }

    public void EliminarExpirados()
    {
        var expirados = _context.RefreshTokens
            .Where(rt => rt.FechaExpiracion < DateTime.Now)
            .ToList();
        _context.RefreshTokens.RemoveRange(expirados);
        _context.SaveChanges();
    }
}