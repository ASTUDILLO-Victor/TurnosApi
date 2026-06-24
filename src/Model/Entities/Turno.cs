namespace TurnosApi.Models.Entities;
public class Turno
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = "Programado";
    public string Motivo { get; set; } = "";

    // Dos foreign keys al mismo Usuario
    public int MedicoId { get; set; }
    public Usuario? Medico { get; set; }

    public int PacienteId { get; set; }
    public Usuario? Paciente { get; set; }
}