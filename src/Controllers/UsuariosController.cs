using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurnosApi.Services.Interfaces;

namespace TurnosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsuariosController : ControllerBase
{
    private readonly IAuthService _service;

    public UsuariosController(IAuthService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetTodos()
    {
        return Ok(_service.ObtenerTodos());
    }

    [HttpPut("{id}/desactivar")]
    public IActionResult Desactivar(int id)
    {
        try
        {
            _service.DesactivarUsuario(id);
            return Ok("Usuario desactivado");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}