using TurnosApi.Models.Entities;

namespace TurnosApi.Repositories.Interfaces;

public interface ITokenBlacklistRepository
{
    void Agregar(TokenBlacklist token);
    bool EstaEnBlacklist(string token);
    void EliminarExpirados();
}