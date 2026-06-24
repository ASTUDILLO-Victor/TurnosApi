using TurnosApi.Models.DTOs;
using TurnosApi.Models.Entities;
using TurnosApi.Repositories.Interfaces;
using TurnosApi.Services.Interfaces;

namespace TurnosApi.Services;

public class TurnoService : ITurnoService
{
    private readonly ITurnoRepository _repository;
    private readonly IUsuarioRepository _usuarioRepository;

    public TurnoService(ITurnoRepository repository, IUsuarioRepository usuarioRepository)
    {
        _repository = repository;
        _usuarioRepository = usuarioRepository;
    }

    public List<TurnoResponseDTO> ObtenerTodos()
    {
        return _repository.ObtenerTodos()
            .Select(MapearTurno).ToList();
    }

    public TurnoResponseDTO? ObtenerPorId(int id)
    {
        var turno = _repository.ObtenerPorId(id);
        if (turno == null) return null;
        return MapearTurno(turno);
    }

    public List<TurnoResponseDTO> ObtenerPorMedico(int medicoId)
    {
        return _repository.ObtenerPorMedico(medicoId)
            .Select(MapearTurno).ToList();
    }

    public List<TurnoResponseDTO> ObtenerPorPaciente(int pacienteId)
    {
        return _repository.ObtenerPorPaciente(pacienteId)
            .Select(MapearTurno).ToList();
    }

    public TurnoResponseDTO Agregar(TurnoCreateDTO dto)
    {
        // Validar que la fecha no sea en el pasado
        if (dto.Fecha < DateTime.UtcNow)
            throw new Exception("No se puede crear un turno en el pasado");

        // Validar que el médico existe
        var medico = _usuarioRepository.ObtenerPorId(dto.MedicoId);
        if (medico == null)
            throw new Exception($"No existe médico con Id {dto.MedicoId}");

        // Validar que el paciente existe
        var paciente = _usuarioRepository.ObtenerPorId(dto.PacienteId);
        if (paciente == null)
            throw new Exception($"No existe paciente con Id {dto.PacienteId}");

        // Validar que el médico no tenga turno en esa fecha
        if (_repository.ExisteTurnoEnFecha(dto.MedicoId, dto.Fecha))
            throw new Exception("El médico ya tiene un turno en esa fecha y hora");

        var turno = new Turno
        {
            Fecha = dto.Fecha,
            Motivo = dto.Motivo,
            MedicoId = dto.MedicoId,
            PacienteId = dto.PacienteId
        };

        var creado = _repository.Agregar(turno);
        var turnoCompleto = _repository.ObtenerPorId(creado.Id)!;
        return MapearTurno(turnoCompleto);
    }

    public TurnoResponseDTO? CambiarEstado(int id, string estado)
    {
        var turno = _repository.ObtenerPorId(id);
        if (turno == null) return null;

        var estadosValidos = new[] { "Programado", "Completado", "Cancelado" };
        if (!estadosValidos.Contains(estado))
            throw new Exception($"Estado inválido. Estados válidos: {string.Join(", ", estadosValidos)}");

        turno.Estado = estado;
        _repository.Actualizar(turno);
        return MapearTurno(turno);
    }

    private TurnoResponseDTO MapearTurno(Turno turno)
    {
        return new TurnoResponseDTO
        {
            Id = turno.Id,
            Fecha = turno.Fecha,
            Motivo = turno.Motivo,
            Estado = turno.Estado,
            MedicoNombre = $"{turno.Medico?.Nombre} {turno.Medico?.Apellido}",
            PacienteNombre = $"{turno.Paciente?.Nombre} {turno.Paciente?.Apellido}"
        };
    }
}