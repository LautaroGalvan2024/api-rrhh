using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitAI.Contratos.Dtos.Coincidencias;
using RecruitAI.Contratos.Interfaces.Servicios;
using RecruitAI.Datos.Entidades;
using RecruitAI.Datos.Persistencia;

namespace RecruitAI.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoincidenciasController : ControllerBase
{
    private readonly CherokeeDbContext _contexto;
    private readonly IEmbeddingsServicio _embeddingsServicio;
    private readonly ICoincidenciasServicio _coincidenciasServicio;

    public CoincidenciasController(
        CherokeeDbContext contexto,
        IEmbeddingsServicio embeddingsServicio,
        ICoincidenciasServicio coincidenciasServicio)
    {
        _contexto = contexto;
        _embeddingsServicio = embeddingsServicio;
        _coincidenciasServicio = coincidenciasServicio;
    }

    [HttpPost("puestos/{puestoId:guid}/embedding")]
    public async Task<IActionResult> GenerarEmbeddingPuestoAsync(Guid puestoId, CancellationToken cancellationToken)
    {
        var puesto = await _contexto.Puestos
            .Include(x => x.EmbeddingPuesto)
            .FirstOrDefaultAsync(x => x.Id == puestoId, cancellationToken);

        if (puesto is null)
        {
            return NotFound();
        }

        var habilidades = puesto.HabilidadesRequeridasJson is null
            ? Array.Empty<string>()
            : JsonSerializer.Deserialize<string[]>(puesto.HabilidadesRequeridasJson) ?? Array.Empty<string>();

        var texto = string.Join(" \n", new[]
        {
            puesto.Titulo,
            puesto.Descripcion,
            puesto.Seniority ?? string.Empty,
            puesto.Ubicacion ?? string.Empty,
            string.Join(", ", habilidades)
        }.Where(x => !string.IsNullOrWhiteSpace(x)));

        var vector = await _embeddingsServicio.GenerarEmbeddingAsync(texto, cancellationToken: cancellationToken);
        var bytes = _embeddingsServicio.ConvertirABytes(vector);

        if (puesto.EmbeddingPuesto is null)
        {
            puesto.EmbeddingPuesto = new EmbeddingPuesto
            {
                PuestoId = puesto.Id,
                Vector = bytes,
                ActualizadoEl = DateTime.UtcNow
            };
        }
        else
        {
            puesto.EmbeddingPuesto.Vector = bytes;
            puesto.EmbeddingPuesto.ActualizadoEl = DateTime.UtcNow;
        }

        await _contexto.SaveChangesAsync(cancellationToken);

        return Ok(new { puestoId, longitud = vector.Length });
    }

    [HttpPost("candidatos/{candidatoId:guid}/embedding")]
    public async Task<IActionResult> GenerarEmbeddingCandidatoAsync(Guid candidatoId, CancellationToken cancellationToken)
    {
        var candidato = await _contexto.Candidatos
            .Include(x => x.EmbeddingCandidato)
            .FirstOrDefaultAsync(x => x.Id == candidatoId, cancellationToken);

        if (candidato is null)
        {
            return NotFound();
        }

        var vector = await _embeddingsServicio.GenerarEmbeddingAsync(candidato.CvTexto, cancellationToken: cancellationToken);
        var bytes = _embeddingsServicio.ConvertirABytes(vector);

        if (candidato.EmbeddingCandidato is null)
        {
            candidato.EmbeddingCandidato = new EmbeddingCandidato
            {
                CandidatoId = candidato.Id,
                Vector = bytes,
                ActualizadoEl = DateTime.UtcNow
            };
        }
        else
        {
            candidato.EmbeddingCandidato.Vector = bytes;
            candidato.EmbeddingCandidato.ActualizadoEl = DateTime.UtcNow;
        }

        await _contexto.SaveChangesAsync(cancellationToken);

        return Ok(new { candidatoId, longitud = vector.Length });
    }

    [HttpGet("puestos/{puestoId:guid}/top")]
    public async Task<ActionResult<IEnumerable<CoincidenciaDto>>> ObtenerTopAsync(Guid puestoId, [FromQuery] TopCoincidenciasRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _coincidenciasServicio.TopPorPuestoAsync(puestoId, request.Cantidad, request.Umbral, cancellationToken);
            return Ok(resultado);
        }
        catch (InvalidOperationException excepcion)
        {
            return BadRequest(new { mensaje = excepcion.Message });
        }
    }
}
