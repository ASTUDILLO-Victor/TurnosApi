using TurnosApi.Models.Entities;

namespace TurnosApi.Repositories.Interfaces;

public interface IRolRepository
{
    Rol? ObtenerPorNombre(string nombre);
    List<Rol> ObtenerTodos();
}