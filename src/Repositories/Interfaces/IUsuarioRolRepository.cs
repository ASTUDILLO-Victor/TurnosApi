using TurnosApi.Models.Entities;

namespace TurnosApi.Repositories.Interfaces;

public interface IUsuarioRolRepository
{
    void Agregar(UsuarioRol usuarioRol);
    List<UsuarioRol> ObtenerPorUsuario(int usuarioId);
}