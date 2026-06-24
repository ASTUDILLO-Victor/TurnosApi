using TurnosApi.Models.Entities;

namespace TurnosApi.Repositories.Interfaces;

public interface ITurnoRepository
{
    List<Turno> ObtenerTodos();
    Turno? ObtenerPorId(int id);
    List<Turno> ObtenerPorMedico(int medicoId);
    List<Turno> ObtenerPorPaciente(int pacienteId);
    bool ExisteTurnoEnFecha(int medicoId, DateTime fecha);
    Turno Agregar(Turno turno);
    Turno Actualizar(Turno turno);
}