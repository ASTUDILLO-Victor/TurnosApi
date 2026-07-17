using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using TurnosApi.Data;
using TurnosApi.Models.Entities;

namespace TurnosApi.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "TestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();

        if (!db.Roles.Any())
        {
            db.Roles.AddRange(
                new Rol { Nombre = "Admin", Descripcion = "Acceso total" },
                new Rol { Nombre = "Medico", Descripcion = "Puede gestionar turnos" },
                new Rol { Nombre = "Paciente", Descripcion = "Puede ver sus turnos" }
            );
            db.SaveChanges();
        }
    }
}