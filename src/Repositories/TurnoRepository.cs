using Microsoft.EntityFrameworkCore;
using TurnosApi.Data;
using TurnosApi.Models.Entities;
using TurnosApi.Repositories.Interfaces;

namespace TurnosApi.Repositories;

public class TurnoRepository : ITurnoRepository
{
    private readonly AppDbContext _context;

    public TurnoRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Turno> ObtenerTodos()
    {
        return _context.Turnos
            .Include(t => t.Medico)
            .Include(t => t.Paciente)
            .ToList();
    }

    public Turno? ObtenerPorId(int id)
    {
        return _context.Turnos
            .Include(t => t.Medico)
            .Include(t => t.Paciente)
            .FirstOrDefault(t => t.Id == id);
    }

    public List<Turno> ObtenerPorMedico(int medicoId)
    {
        return _context.Turnos
            .Include(t => t.Medico)
            .Include(t => t.Paciente)
            .Where(t => t.MedicoId == medicoId)
            .ToList();
    }

    public List<Turno> ObtenerPorPaciente(int pacienteId)
    {
        return _context.Turnos
            .Include(t => t.Medico)
            .Include(t => t.Paciente)
            .Where(t => t.PacienteId == pacienteId)
            .ToList();
    }

    public bool ExisteTurnoEnFecha(int medicoId, DateTime fecha)
    {
        return _context.Turnos.Any(t =>
            t.MedicoId == medicoId &&
            t.Fecha == fecha &&
            t.Estado == "Programado");
    }

    public Turno Agregar(Turno turno)
    {
        _context.Turnos.Add(turno);
        _context.SaveChanges();
        return turno;
    }

    public Turno Actualizar(Turno turno)
    {
        _context.Turnos.Update(turno);
        _context.SaveChanges();
        return turno;
    }
}