namespace TurnosApi.Models.DTOs;

public class TurnoCreateDTO
{
    public DateTime Fecha { get; set; }
    public string Motivo { get; set; } = "";
    public int MedicoId { get; set; }
    public int PacienteId { get; set; }
}

public class TurnoResponseDTO
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Motivo { get; set; } = "";
    public string Estado { get; set; } = "";
    public string MedicoNombre { get; set; } = "";
    public string PacienteNombre { get; set; } = "";
}