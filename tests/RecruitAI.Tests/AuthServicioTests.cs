using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using RecruitAI.Contratos.Dtos.Autenticacion;
using RecruitAI.Servicios.Implementaciones;
using Xunit;

namespace RecruitAI.Tests;

public class AuthServicioTests
{
    [Fact]
    public async Task GenerarTokenAsync_CreaTokenConRol()
    {
        var servicio = new AuthServicio(CrearConfiguracion());

        var respuesta = await servicio.GenerarTokenAsync(new LoginRequest
        {
            Usuario = "admin",
            Contrasena = "Admin123!"
        });

        Assert.NotNull(respuesta);
        Assert.Equal("Administrador", respuesta.Rol);
        Assert.False(string.IsNullOrWhiteSpace(respuesta.Token));

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(respuesta.Token);
        Assert.Contains(token.Claims, claim => claim.Type == "role" && claim.Value == "Administrador");
    }

    [Fact]
    public async Task GenerarTokenAsync_CredencialesInvalidasLanzaExcepcion()
    {
        var servicio = new AuthServicio(CrearConfiguracion());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => servicio.GenerarTokenAsync(new LoginRequest
        {
            Usuario = "admin",
            Contrasena = "incorrecta"
        }));
    }

    private static IConfiguration CrearConfiguracion()
    {
        var valores = new Dictionary<string, string?>
        {
            ["Jwt:Emisor"] = "http://localhost",
            ["Jwt:Audiencia"] = "http://localhost",
            ["Jwt:Secreto"] = "clave-muy-segura",
            ["Jwt:ExpiracionMinutos"] = "60",
            ["Usuarios:0:Usuario"] = "admin",
            ["Usuarios:0:Contrasena"] = "Admin123!",
            ["Usuarios:0:Rol"] = "Administrador"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(valores)
            .Build();
    }
}
