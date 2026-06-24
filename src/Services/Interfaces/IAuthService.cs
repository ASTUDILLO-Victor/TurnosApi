using TurnosApi.Models.DTOs;

namespace TurnosApi.Services.Interfaces;

public interface IAuthService
{
    AuthResponseDTO Registro(RegistroDTO dto);
    AuthResponseDTO Login(LoginDTO dto);
    void Logout(string accessToken, string refreshToken);
    AuthResponseDTO Refresh(string refreshToken);
    PerfilResponseDTO ObtenerPerfil(int usuarioId);
    PerfilResponseDTO ActualizarPerfil(int usuarioId, ActualizarPerfilDTO dto);
    void CambiarPassword(int usuarioId, CambiarPasswordDTO dto);
    List<PerfilResponseDTO> ObtenerTodos();
    void DesactivarUsuario(int id);
}