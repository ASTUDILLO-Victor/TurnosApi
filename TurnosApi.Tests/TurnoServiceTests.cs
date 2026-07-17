using Moq;
using Xunit;
using TurnosApi.Services;
using TurnosApi.Repositories.Interfaces;
using TurnosApi.Models.DTOs;
using TurnosApi.Models.Entities;

namespace TurnosApi.Tests;

public class TurnoServiceTests
{
    [Fact]
    public void Agregar_FechaEnElPasado_LanzaExcepcion()
    {
        // Arrange
        var mockTurnoRepo = new Mock<ITurnoRepository>();
        var mockUsuarioRepo = new Mock<IUsuarioRepository>();

        var service = new TurnoService(mockTurnoRepo.Object, mockUsuarioRepo.Object);

        var dto = new TurnoCreateDTO
        {
            Fecha = DateTime.UtcNow.AddDays(-1), // ayer — fecha pasada
            Motivo = "Consulta general",
            MedicoId = 1,
            PacienteId = 2
        };

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => service.Agregar(dto));
        Assert.Equal("No se puede crear un turno en el pasado", exception.Message);
    }

    [Fact]
    public void Agregar_MedicoNoExiste_LanzaExcepcion()
    {
        // Arrange
        var mockTurnoRepo = new Mock<ITurnoRepository>();
        var mockUsuarioRepo = new Mock<IUsuarioRepository>();
        
        // Aquí necesitas configurar el mock para que ObtenerPorId retorne null
        mockUsuarioRepo.Setup(r => r.ObtenerPorId(It.IsAny<int>())).Returns((Usuario?)null);
        
        var service = new TurnoService(mockTurnoRepo.Object, mockUsuarioRepo.Object);

        var dto = new TurnoCreateDTO
        {
            Fecha = DateTime.UtcNow.AddDays(1), // fecha futura válida
            Motivo = "Consulta",
            MedicoId = 999,
            PacienteId = 2
        };

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => service.Agregar(dto));
        Assert.Equal("No existe médico con Id 999", exception.Message);
    }
    [Fact]
    public void Agregar_PacienteNoExiste_LanzaExcepcion()
    {
        // Arrange
        var mockTurnoRepo = new Mock<ITurnoRepository>();
        var mockUsuarioRepo = new Mock<IUsuarioRepository>();

        // Aquí necesitas configurar el mock para que ObtenerPorId retorne null
        mockUsuarioRepo.Setup(r => r.ObtenerPorId(1)).Returns(new Usuario { Id = 1, Nombre = "Dr. García" });
        mockUsuarioRepo.Setup(r => r.ObtenerPorId(999)).Returns((Usuario?)null);

        var service = new TurnoService(mockTurnoRepo.Object, mockUsuarioRepo.Object);

        var dto = new TurnoCreateDTO
        {
            Fecha = DateTime.UtcNow.AddDays(1), // fecha futura válida
            Motivo = "Consulta",
            MedicoId = 1,
            PacienteId = 999
        };

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => service.Agregar(dto));
        Assert.Equal("No existe paciente con Id 999", exception.Message);
    }
    [Fact]
    public void Agregar_FechaYaExiste_LanzaExcepcion()
    {
        // Arrange
        var mockTurnoRepo = new Mock<ITurnoRepository>();
        var mockUsuarioRepo = new Mock<IUsuarioRepository>();

        // Aquí necesitas configurar el mock para que ObtenerPorId retorne null
        mockUsuarioRepo.Setup(r => r.ObtenerPorId(1)).Returns(new Usuario { Id = 1, Nombre = "Dr. García" });
        mockUsuarioRepo.Setup(r => r.ObtenerPorId(999)).Returns(new Usuario { Id = 1, Nombre = "victor" });
        mockTurnoRepo.Setup(r => r.ExisteTurnoEnFecha(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);

        var service = new TurnoService(mockTurnoRepo.Object, mockUsuarioRepo.Object);

        var dto = new TurnoCreateDTO
        {
            Fecha = DateTime.UtcNow.AddDays(1), // fecha futura válida
            Motivo = "Consulta",
            MedicoId = 1,
            PacienteId = 999
        };

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => service.Agregar(dto));
        Assert.Equal("El médico ya tiene un turno en esa fecha y hora", exception.Message);
    }
    [Fact]
    public void Agregar_DatosValidos_CreaTurnoCorrectamente()
    {
        // Arrange
        var mockTurnoRepo = new Mock<ITurnoRepository>();
        var mockUsuarioRepo = new Mock<IUsuarioRepository>();

        var medico = new Usuario { Id = 1, Nombre = "Dr. García" };
        var paciente = new Usuario { Id = 2, Nombre = "Victor" };

        mockUsuarioRepo.Setup(r => r.ObtenerPorId(1)).Returns(medico);
        mockUsuarioRepo.Setup(r => r.ObtenerPorId(2)).Returns(paciente);
        mockTurnoRepo.Setup(r => r.ExisteTurnoEnFecha(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

        var turnoCreado = new Turno { Id = 1, MedicoId = 1, PacienteId = 2, Motivo = "Consulta" };
        mockTurnoRepo.Setup(r => r.Agregar(It.IsAny<Turno>())).Returns(turnoCreado);
        mockTurnoRepo.Setup(r => r.ObtenerPorId(1)).Returns(turnoCreado);

        var service = new TurnoService(mockTurnoRepo.Object, mockUsuarioRepo.Object);

        var dto = new TurnoCreateDTO
        {
            Fecha = DateTime.UtcNow.AddDays(1),
            Motivo = "Programada",
            MedicoId = 1,
            PacienteId = 2
        };

        // Act
        var resultado = service.Agregar(dto);

        // Assert
        Assert.NotNull(resultado);
        //Assert.Equal("Consulta", resultado.Motivo);
        Assert.Equal("Esto va a fallar", resultado.Motivo);
    }
    [Fact]
    public void CambiarEstado_TurnoExisteYEstadoValido_ActualizaCorrectamente()
    {
        // Arrange
        var mockTurnoRepo = new Mock<ITurnoRepository>();
        var mockUsuarioRepo = new Mock<IUsuarioRepository>();

        var turnoExistente = new Turno { Id = 1, MedicoId = 1, PacienteId = 2, Estado = "Programada" };
        mockTurnoRepo.Setup(r => r.ObtenerPorId(1)).Returns(turnoExistente);

        var service = new TurnoService(mockTurnoRepo.Object, mockUsuarioRepo.Object);

        // Act
        var resultado = service.CambiarEstado(1, "Completado");

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Completado", resultado.Estado);
    }
}