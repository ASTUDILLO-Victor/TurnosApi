using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TurnosApi.Models.DTOs;
using TurnosApi.Models.Entities;
using TurnosApi.Repositories.Interfaces;
using TurnosApi.Services.Interfaces;

namespace TurnosApi.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenBlacklistRepository _blacklistRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IUsuarioRolRepository _usuarioRolRepository;
    private readonly IConfiguration _configuration;

    
    public AuthService(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenBlacklistRepository blacklistRepository,
        IRolRepository rolRepository,
        IUsuarioRolRepository usuarioRolRepository,
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _blacklistRepository = blacklistRepository;
        _rolRepository = rolRepository;
        _usuarioRolRepository = usuarioRolRepository;
        _configuration = configuration;
    }

    public AuthResponseDTO Registro(RegistroDTO dto)
    {
        if (dto.Password != dto.ConfirmarPassword)
            throw new Exception("Las contraseñas no coinciden");

        if (_usuarioRepository.ExisteEmail(dto.Email))
            throw new Exception("El email ya está registrado");

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email,
            Telefono = dto.Telefono,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Activo = true,
            FechaRegistro = DateTime.UtcNow
        };

        var creado = _usuarioRepository.Agregar(usuario);

        // Asignar rol Cliente desde BD
        var rolCliente = _rolRepository.ObtenerPorNombre("Cliente");
        if (rolCliente == null)
            throw new Exception("Rol Cliente no encontrado en BD");

        _usuarioRolRepository.Agregar(new UsuarioRol
        {
            UsuarioId = creado.Id,
            RolId = rolCliente.Id
        });

        // Recargar usuario con roles
        var usuarioConRoles = _usuarioRepository.ObtenerPorId(creado.Id)!;
        return GenerarAuthResponse(usuarioConRoles);
    }

    public AuthResponseDTO Login(LoginDTO dto)
    {
        var usuario = _usuarioRepository.ObtenerPorEmail(dto.Email);
        if (usuario == null)
            throw new Exception("Email o contraseña incorrectos");

        if (!usuario.Activo)
            throw new Exception("Usuario desactivado. Contacte al administrador");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            throw new Exception("Email o contraseña incorrectos");

        usuario.UltimoLogin = DateTime.UtcNow;
        _usuarioRepository.Actualizar(usuario);

        return GenerarAuthResponse(usuario);
    }

    public void Logout(string accessToken, string refreshToken)
    {
        // Agregar access token a blacklist
        var expiracion = ObtenerExpiracionToken(accessToken);
        _blacklistRepository.Agregar(new TokenBlacklist
        {
            Token = accessToken,
            FechaExpiracion = expiracion
        });

        // Revocar refresh token
        var rt = _refreshTokenRepository.ObtenerPorToken(refreshToken);
        if (rt != null)
            _refreshTokenRepository.Revocar(rt);
    }

    public AuthResponseDTO Refresh(string refreshToken)
    {
        var rt = _refreshTokenRepository.ObtenerPorToken(refreshToken);

        if (rt == null)
            throw new Exception("Refresh token inválido");

        if (rt.EstaRevocado)
            throw new Exception("Refresh token revocado");

        if (rt.FechaExpiracion < DateTime.Now)
            throw new Exception("Refresh token expirado");

        var usuario = _usuarioRepository.ObtenerPorId(rt.UsuarioId);
        if (usuario == null)
            throw new Exception("Usuario no encontrado");

        // Revocar el refresh token actual
        _refreshTokenRepository.Revocar(rt);

        // Generar nueva respuesta con nuevos tokens
        return GenerarAuthResponse(usuario);
    }

    public PerfilResponseDTO ObtenerPerfil(int usuarioId)
    {
        var usuario = _usuarioRepository.ObtenerPorId(usuarioId);
        if (usuario == null)
            throw new Exception("Usuario no encontrado");

        return MapearPerfil(usuario);
    }

    public PerfilResponseDTO ActualizarPerfil(int usuarioId, ActualizarPerfilDTO dto)
    {
        var usuario = _usuarioRepository.ObtenerPorId(usuarioId);
        if (usuario == null)
            throw new Exception("Usuario no encontrado");

        usuario.Nombre = dto.Nombre;
        usuario.Apellido = dto.Apellido;
        usuario.Telefono = dto.Telefono;

        var actualizado = _usuarioRepository.Actualizar(usuario);
        return MapearPerfil(actualizado);
    }

    public void CambiarPassword(int usuarioId, CambiarPasswordDTO dto)
    {
        if (dto.NuevoPassword != dto.ConfirmarPassword)
            throw new Exception("Las contraseñas no coinciden");

        var usuario = _usuarioRepository.ObtenerPorId(usuarioId);
        if (usuario == null)
            throw new Exception("Usuario no encontrado");

        if (!BCrypt.Net.BCrypt.Verify(dto.PasswordActual, usuario.PasswordHash))
            throw new Exception("La contraseña actual es incorrecta");

        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NuevoPassword);
        _usuarioRepository.Actualizar(usuario);
    }

    public List<PerfilResponseDTO> ObtenerTodos()
    {
        return _usuarioRepository.ObtenerTodos()
            .Select(MapearPerfil).ToList();
    }

    public void DesactivarUsuario(int id)
    {
        var usuario = _usuarioRepository.ObtenerPorId(id);
        if (usuario == null)
            throw new Exception("Usuario no encontrado");

        usuario.Activo = false;
        _usuarioRepository.Actualizar(usuario);
    }

    // Métodos privados
    private AuthResponseDTO GenerarAuthResponse(Usuario usuario)
    {
        string accessToken = GenerarAccessToken(usuario);
        string refreshToken = GenerarRefreshToken();

        int refreshDays = int.Parse(_configuration["Jwt:RefreshTokenDays"]!);

        _refreshTokenRepository.Agregar(new RefreshToken
        {
            Token = refreshToken,
            UsuarioId = usuario.Id,
            FechaExpiracion = DateTime.UtcNow.AddDays(refreshDays)
        });

        return new AuthResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.UsuarioRoles.FirstOrDefault()?.Rol?.Nombre ?? "Cliente",
            UltimoLogin = usuario.UltimoLogin
        };
    }

   private string GenerarAccessToken(Usuario usuario)
    {
        var claimsList = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.UsuarioRoles
                .FirstOrDefault()?.Rol?.Nombre ?? "Cliente")
        };

        // Agregar permisos del usuario al token
        var permisos = usuario.UsuarioRoles
            .SelectMany(ur => ur.Rol!.RolPermisos)
            .Select(rp => rp.Permiso!.Nombre)
            .Distinct();

        foreach (var permiso in permisos)
        {
            claimsList.Add(new Claim("permiso", permiso));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        int minutes = int.Parse(_configuration["Jwt:AccessTokenMinutes"]!);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claimsList,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerarRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private DateTime ObtenerExpiracionToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        return jwt.ValidTo;
    }

    private PerfilResponseDTO MapearPerfil(Usuario usuario)
    {
        return new PerfilResponseDTO
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Telefono = usuario.Telefono,
            Rol = usuario.UsuarioRoles.FirstOrDefault()?.Rol?.Nombre ?? "Cliente",
            FechaRegistro = usuario.FechaRegistro,
            UltimoLogin = usuario.UltimoLogin
        };
    }

    
}