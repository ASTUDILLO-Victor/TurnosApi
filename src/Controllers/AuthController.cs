using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TurnosApi.Models.DTOs;
using TurnosApi.Services.Interfaces;

namespace TurnosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("registro")]
    public IActionResult Registro([FromBody] RegistroDTO dto)
    {
        try
        {
            var resultado = _service.Registro(dto);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            // Muestra el error completo incluyendo el inner exception
            return BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO dto)
    {
        try
        {
            var resultado = _service.Login(dto);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout([FromBody] RefreshTokenDTO dto)
    {
        try
        {
            // Obtener el access token del header
            string accessToken = Request.Headers["Authorization"]
                .ToString().Replace("Bearer ", "");

            _service.Logout(accessToken, dto.RefreshToken);
            return Ok("Sesión cerrada correctamente");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshTokenDTO dto)
    {
        try
        {
            var resultado = _service.Refresh(dto.RefreshToken);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("perfil")]
    [Authorize]
    public IActionResult ObtenerPerfil()
    {
        try
        {
            int usuarioId = ObtenerUsuarioId();
            var perfil = _service.ObtenerPerfil(usuarioId);
            return Ok(perfil);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("perfil")]
    [Authorize]
    public IActionResult ActualizarPerfil([FromBody] ActualizarPerfilDTO dto)
    {
        try
        {
            int usuarioId = ObtenerUsuarioId();
            var perfil = _service.ActualizarPerfil(usuarioId, dto);
            return Ok(perfil);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("cambiar-password")]
    [Authorize]
    public IActionResult CambiarPassword([FromBody] CambiarPasswordDTO dto)
    {
        try
        {
            int usuarioId = ObtenerUsuarioId();
            _service.CambiarPassword(usuarioId, dto);
            return Ok("Contraseña cambiada correctamente");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Método privado para obtener el Id del usuario del token
    private int ObtenerUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
            throw new Exception("Usuario no autenticado");
        return int.Parse(claim.Value);
    }
}