using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TurnosApi.Models.DTOs;
using TurnosApi.Services.Interfaces;

namespace TurnosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TurnosController : ControllerBase
{
    private readonly ITurnoService _service;

    public TurnosController(ITurnoService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "ver_turnos")]
    public IActionResult GetTodos()
    {
        return Ok(_service.ObtenerTodos());
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "ver_turnos")]
    public IActionResult GetPorId(int id)
    {
        var turno = _service.ObtenerPorId(id);
        if (turno == null)
            return NotFound($"No existe turno con Id {id}");
        return Ok(turno);
    }

    [HttpGet("medico/{medicoId}")]
    [Authorize(Policy = "ver_turnos")]
    public IActionResult GetPorMedico(int medicoId)
    {
        var turnos = _service.ObtenerPorMedico(medicoId);
        if (turnos.Count == 0)
            return NotFound($"No hay turnos para el médico {medicoId}");
        return Ok(turnos);
    }

    [HttpGet("mis-turnos")]
    [Authorize(Policy = "ver_turnos")]
    public IActionResult GetMisTurnos()
    {
        int usuarioId = ObtenerUsuarioId();
        var turnos = _service.ObtenerPorPaciente(usuarioId);
        return Ok(turnos);
    }

    [HttpPost]
    [Authorize(Policy = "gestionar_turnos")]
    public IActionResult Agregar([FromBody] TurnoCreateDTO dto)
    {
        try
        {
            var creado = _service.Agregar(dto);
            return Created($"/api/turnos/{creado.Id}", creado);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/estado")]
    [Authorize(Policy = "gestionar_turnos")]
    public IActionResult CambiarEstado(int id, [FromBody] string estado)
    {
        try
        {
            var turno = _service.CambiarEstado(id, estado);
            if (turno == null)
                return NotFound($"No existe turno con Id {id}");
            return Ok(turno);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private int ObtenerUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
            throw new Exception("Usuario no autenticado");
        return int.Parse(claim.Value);
    }
}