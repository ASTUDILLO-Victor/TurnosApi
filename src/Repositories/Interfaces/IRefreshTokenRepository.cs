using TurnosApi.Models.Entities;

namespace TurnosApi.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    RefreshToken Agregar(RefreshToken refreshToken);
    RefreshToken? ObtenerPorToken(string token);
    void Revocar(RefreshToken refreshToken);
    void EliminarExpirados();
}