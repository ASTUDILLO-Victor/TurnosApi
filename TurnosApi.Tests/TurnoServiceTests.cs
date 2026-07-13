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
}