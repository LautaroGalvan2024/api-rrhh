using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitAI.Contratos.Constantes;
using RecruitAI.Contratos.Dtos.Candidatos;
using RecruitAI.Datos.Entidades;
using RecruitAI.Datos.Persistencia;

namespace RecruitAI.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CandidatosController : ControllerBase
{
    private readonly CherokeeDbContext _contextoEscritura;
    private readonly IDbContextFactory<CherokeeDbContext> _contextoLecturaFactory;

    public CandidatosController(
        CherokeeDbContext contextoEscritura,
        IDbContextFactory<CherokeeDbContext> contextoLecturaFactory)
    {
        _contextoEscritura = contextoEscritura;
        _contextoLecturaFactory = contextoLecturaFactory;
    }

    [Authorize(Policy = "lectura")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CandidatoDto>>> ObtenerAsync(CancellationToken cancellationToken)
    {
        await using var contextoLectura = await _contextoLecturaFactory.CreateDbContextAsync(cancellationToken);
        var candidatos = await contextoLectura.Candidatos
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var resultado = candidatos.Select(MapearCandidato).ToList();
        return Ok(resultado);
    }

    [Authorize(Policy = "lectura")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CandidatoDto>> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var contextoLectura = await _contextoLecturaFactory.CreateDbContextAsync(cancellationToken);
        var candidato = await contextoLectura.Candidatos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (candidato is null)
        {
            return NotFound();
        }

        return Ok(MapearCandidato(candidato));
    }

    [Authorize(Policy = "administracion")]
    [HttpPost]
    public async Task<ActionResult<CandidatoDto>> CrearAsync([FromBody] CrearCandidatoDto dto, CancellationToken cancellationToken)
    {
        var candidato = new Candidato
        {
            Id = Guid.NewGuid(),
            NombreCompleto = dto.NombreCompleto,
            Email = dto.Email,
            Fuente = dto.Fuente,
            CvTexto = dto.CvTexto,
            CreadoEl = DateTime.UtcNow
        };

        await _contextoEscritura.Candidatos.AddAsync(candidato, cancellationToken);
        await _contextoEscritura.SaveChangesAsync(cancellationToken);

        var resultado = MapearCandidato(candidato);
        return CreatedAtAction(nameof(ObtenerPorIdAsync), new { id = candidato.Id }, resultado);
    }

    [Authorize(Policy = "administracion")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CandidatoDto>> ActualizarAsync(Guid id, [FromBody] EditarCandidatoDto dto, CancellationToken cancellationToken)
    {
        var candidato = await _contextoEscritura.Candidatos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (candidato is null)
        {
            return NotFound();
        }

        candidato.NombreCompleto = dto.NombreCompleto;
        candidato.Email = dto.Email;
        candidato.Fuente = dto.Fuente;
        candidato.CvTexto = dto.CvTexto;

        await _contextoEscritura.SaveChangesAsync(cancellationToken);

        return Ok(MapearCandidato(candidato));
    }

    [Authorize(Policy = "administracion")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> EliminarAsync(Guid id, CancellationToken cancellationToken)
    {
        var candidato = await _contextoEscritura.Candidatos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (candidato is null)
        {
            return NotFound();
        }

        _contextoEscritura.Candidatos.Remove(candidato);
        await _contextoEscritura.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static CandidatoDto MapearCandidato(Candidato candidato)
    {
        return new CandidatoDto
        {
            Id = candidato.Id,
            NombreCompleto = candidato.NombreCompleto,
            Email = candidato.Email,
            Fuente = candidato.Fuente,
            CvTexto = candidato.CvTexto,
            CreadoEl = candidato.CreadoEl
        };
    }
}
