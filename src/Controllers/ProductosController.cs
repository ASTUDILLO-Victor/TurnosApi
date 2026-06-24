using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetTodos()
    {
        return Ok("Lista de productos — acceso público");
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "ver_productos")]
    public IActionResult GetPorId(int id)
    {
        return Ok($"Producto {id} — tienes permiso ver_productos");
    }

    [HttpPost]
    [Authorize(Policy = "gestionar_productos")]
    public IActionResult Crear()
    {
        return Ok("Producto creado — tienes permiso gestionar_productos");
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "gestionar_productos")]
    public IActionResult Eliminar(int id)
    {
        return Ok($"Producto {id} eliminado — tienes permiso gestionar_productos");
    }
}