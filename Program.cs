using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TurnosApi.Data;
using TurnosApi.Repositories;
using TurnosApi.Repositories.Interfaces;
using TurnosApi.Services;
using TurnosApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// PostgreSQL
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// JWT
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ver_turnos", policy =>
        policy.RequireClaim("permiso", "ver_turnos"));

    options.AddPolicy("gestionar_turnos", policy =>
        policy.RequireClaim("permiso", "gestionar_turnos"));

    options.AddPolicy("ver_medicos", policy =>
        policy.RequireClaim("permiso", "ver_medicos"));

    options.AddPolicy("gestionar_medicos", policy =>
        policy.RequireClaim("permiso", "gestionar_medicos"));

    options.AddPolicy("ver_pacientes", policy =>
        policy.RequireClaim("permiso", "ver_pacientes"));

    options.AddPolicy("gestionar_pacientes", policy =>
        policy.RequireClaim("permiso", "gestionar_pacientes"));
});

// Repositorios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ITokenBlacklistRepository, TokenBlacklistRepository>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IUsuarioRolRepository, UsuarioRolRepository>();
builder.Services.AddScoped<ITurnoRepository, TurnoRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITurnoService, TurnoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();