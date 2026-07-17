using System.Net;
using System.Net.Http.Json;
//using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TurnosApi.IntegrationTests;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory> //IClassFixture<WebApplicationFactory<Program>> //Le dice a xUnit: "levanta la API completa (con su Program.cs real) antes de correr estos tests".
{
    //private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    /*public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();//Crea un cliente HTTP que puede hacer requests reales a tu API en memoria — como si fuera Postman pero automatizado.
    }*/
    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Registro_DatosValidos_Retorna200()
    {
        // Arrange
        var registroDto = new
        {
            nombre = "Test",
            apellido = "Usuario",
            email = $"test{Guid.NewGuid()}@mail.com", // email único cada vez
            telefono = "0991234567",
            password = "Test123456!",
            confirmarPassword = "Test123456!"
        ;

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/registro", registroDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}