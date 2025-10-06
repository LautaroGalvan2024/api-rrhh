using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RecruitAI.Contratos.Configuracion;
using RecruitAI.Contratos.Dtos.Autenticacion;
using RecruitAI.Contratos.Interfaces.Servicios;

namespace RecruitAI.Servicios.Implementaciones;

public class AuthServicio : IAuthServicio
{
    private readonly IConfiguration _configuration;

    public AuthServicio(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<TokenResponse> GenerarTokenAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var usuarios = _configuration.GetSection("Usuarios").Get<List<UsuarioConfiguracion>>()
            ?? new List<UsuarioConfiguracion>();

        var usuario = usuarios.FirstOrDefault(x =>
            string.Equals(x.Usuario, request.Usuario, StringComparison.OrdinalIgnoreCase));

        if (usuario is null || string.IsNullOrWhiteSpace(usuario.Contrasena) ||
            !string.Equals(usuario.Contrasena, request.Contrasena))
        {
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        var jwtOptions = _configuration.GetSection("Jwt").Get<JwtOptions>()
            ?? throw new InvalidOperationException("No se configuró la sección Jwt en appsettings.json.");

        if (string.IsNullOrWhiteSpace(jwtOptions.Secreto))
        {
            throw new InvalidOperationException("El secreto JWT no está configurado.");
        }

        var ahora = DateTime.UtcNow;
        var expiracion = ahora.AddMinutes(jwtOptions.ExpiracionMinutos <= 0 ? 60 : jwtOptions.ExpiracionMinutos);

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secreto)),
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, request.Usuario),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, request.Usuario),
            new(ClaimTypes.Role, usuario.Rol)
        };

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Emisor,
            audience: jwtOptions.Audiencia,
            claims: claims,
            notBefore: ahora,
            expires: expiracion,
            signingCredentials: credenciales);

        var handler = new JwtSecurityTokenHandler();
        var tokenSerializado = handler.WriteToken(token);

        var respuesta = new TokenResponse
        {
            Token = tokenSerializado,
            ExpiraEn = expiracion,
            Rol = usuario.Rol
        };

        return Task.FromResult(respuesta);
    }
}
