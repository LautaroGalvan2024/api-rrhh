using Microsoft.AspNetCore.Mvc;
using RecruitAI.Contratos.Dtos.Ia;
using RecruitAI.Contratos.Interfaces.Servicios;

namespace RecruitAI.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IaController : ControllerBase
{
    private readonly IIaServicio _iaServicio;

    public IaController(IIaServicio iaServicio)
    {
        _iaServicio = iaServicio;
    }

    [HttpPost("extraer-cv")]
    public async Task<ActionResult<string>> ExtraerCvAsync([FromBody] ExtraerCvRequest request, CancellationToken cancellationToken)
    {
        var resultado = await _iaServicio.ExtraerCvAsync(request, cancellationToken);
        return Ok(resultado);
    }

    [HttpPost("puntuar")]
    public async Task<ActionResult<double>> PuntuarAsync([FromBody] PuntuarRequest request, CancellationToken cancellationToken)
    {
        var resultado = await _iaServicio.PuntuarAsync(request, cancellationToken);
        return Ok(resultado);
    }
}
