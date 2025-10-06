using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitAI.Contratos.Dtos.Autenticacion;
using RecruitAI.Contratos.Interfaces.Servicios;

namespace RecruitAI.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthServicio _authServicio;

    public AuthController(IAuthServicio authServicio)
    {
        _authServicio = authServicio;
    }

    [AllowAnonymous]
    [HttpPost("token")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> GenerarTokenAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _authServicio.GenerarTokenAsync(request, cancellationToken);
            return Ok(token);
        }
        catch (UnauthorizedAccessException excepcion)
        {
            return Unauthorized(new { mensaje = excepcion.Message });
        }
        catch (InvalidOperationException excepcion)
        {
            return Problem(excepcion.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
