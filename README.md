# 🏥 TurnosApi

API REST para la gestión de turnos médicos, construida con **ASP.NET Core** y **PostgreSQL**, con un sistema completo de autenticación y autorización basado en roles y permisos gestionados desde base de datos.

## ✨ Características principales

- 🔐 **Autenticación JWT completa**: Access Token, Refresh Token y Blacklist para logout seguro
- 👥 **Roles y permisos dinámicos**: gestionados desde base de datos, no hardcodeados en el código
- 🏗️ **Arquitectura en capas**: Controller → Service → Repository, con interfaces e inyección de dependencias
- 📅 **Gestión de turnos médicos**: creación, consulta y cambio de estado con reglas de negocio reales
- ✅ **Unit Tests**: cobertura de la lógica de negocio con xUnit y Moq
- 🐘 **PostgreSQL** con Entity Framework Core

## 🛠️ Stack tecnológico

| Tecnología | Uso |
|---|---|
| ASP.NET Core 10 | Framework web / API REST |
| Entity Framework Core | ORM |
| PostgreSQL | Base de datos |
| JWT Bearer | Autenticación |
| BCrypt.Net | Encriptación de contraseñas |
| xUnit + Moq | Testing |
| Swagger | Documentación interactiva de la API |

## 📋 Funcionalidades

### Autenticación
- Registro y login con contraseñas encriptadas (BCrypt)
- Access Token (15 min) + Refresh Token (7 días) con rotación
- Logout con invalidación de tokens (blacklist)
- Roles: **Admin**, **Medico**, **Paciente**

### Turnos médicos
- Crear turno validando disponibilidad del médico
- Prevención de doble reserva en la misma fecha/hora
- Cambio de estado (Programada, Completada, Cancelada)
- Consulta de turnos propios y por médico

## 🚀 Cómo ejecutarlo

### Requisitos
- .NET 10 SDK
- PostgreSQL

### Pasos

\`\`\`bash
git clone https://github.com/ASTUDILLO-Victor/TurnosApi.git
cd TurnosApi

# Configurar la cadena de conexión
# Crea appsettings.Development.json con tu contraseña de PostgreSQL

# Aplicar migrations
dotnet ef database update

# Ejecutar
dotnet run
\`\`\`

Abre `http://localhost:PUERTO/swagger` para probar los endpoints.

### Ejecutar los tests

\`\`\`bash
cd TurnosApi.Tests
dotnet test
\`\`\`

## 📁 Estructura del proyecto

\`\`\`
TurnosApi/
├── src/
│   ├── Controllers/
│   ├── Services/
│   ├── Repositories/
│   ├── Models/
│   │   ├── Entities/
│   │   └── DTOs/
│   └── Data/
├── TurnosApi.Tests/
├── Program.cs
└── TurnosApi.csproj
\`\`\`

## 🔑 Endpoints principales

| Verbo | Endpoint | Acceso |
|---|---|---|
| POST | `/api/auth/registro` | Público |
| POST | `/api/auth/login` | Público |
| POST | `/api/auth/logout` | Autenticado |
| POST | `/api/auth/refresh` | Público |
| GET | `/api/turnos` | ver_turnos |
| POST | `/api/turnos` | gestionar_turnos |
| PUT | `/api/turnos/{id}/estado` | gestionar_turnos |
| GET | `/api/turnos/mis-turnos` | Autenticado |

## 👤 Autor

**Victor Astudillo**
[GitHub](https://github.com/ASTUDILLO-Victor)
