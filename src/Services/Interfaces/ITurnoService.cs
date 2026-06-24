using TurnosApi.Models.DTOs;

namespace TurnosApi.Services.Interfaces;

public interface ITurnoService
{
    List<TurnoResponseDTO> ObtenerTodos();
    TurnoResponseDTO? ObtenerPorId(int id);
    List<TurnoResponseDTO> ObtenerPorMedico(int medicoId);
    List<TurnoResponseDTO> ObtenerPorPaciente(int pacienteId);
    TurnoResponseDTO Agregar(TurnoCreateDTO dto);
    TurnoResponseDTO? CambiarEstado(int id, string estado);
}