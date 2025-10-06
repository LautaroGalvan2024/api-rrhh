using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitAI.Contratos.Dtos.Candidatos;
using RecruitAI.Datos.Entidades;
using RecruitAI.Datos.Persistencia;

namespace RecruitAI.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidatosController : ControllerBase
{
    private readonly CherokeeDbContext _contexto;
    private readonly CherokeeDbContextLectura _contextoLectura;

    public CandidatosController(CherokeeDbContext contexto, CherokeeDbContextLectura contextoLectura)
    {
        _contexto = contexto;
        _contextoLectura = contextoLectura;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CandidatoDto>>> ObtenerAsync(CancellationToken cancellationToken)
    {
        var candidatos = await _contextoLectura.Candidatos
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var resultado = candidatos.Select(MapearCandidato).ToList();
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CandidatoDto>> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var candidato = await _contextoLectura.Candidatos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (candidato is null)
        {
            return NotFound();
        }

        return Ok(MapearCandidato(candidato));
    }

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

        await _contexto.Candidatos.AddAsync(candidato, cancellationToken);
        await _contexto.SaveChangesAsync(cancellationToken);

        var resultado = MapearCandidato(candidato);
        return CreatedAtAction(nameof(ObtenerPorIdAsync), new { id = candidato.Id }, resultado);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CandidatoDto>> ActualizarAsync(Guid id, [FromBody] EditarCandidatoDto dto, CancellationToken cancellationToken)
    {
        var candidato = await _contexto.Candidatos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (candidato is null)
        {
            return NotFound();
        }

        candidato.NombreCompleto = dto.NombreCompleto;
        candidato.Email = dto.Email;
        candidato.Fuente = dto.Fuente;
        candidato.CvTexto = dto.CvTexto;

        await _contexto.SaveChangesAsync(cancellationToken);

        return Ok(MapearCandidato(candidato));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> EliminarAsync(Guid id, CancellationToken cancellationToken)
    {
        var candidato = await _contexto.Candidatos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (candidato is null)
        {
            return NotFound();
        }

        _contexto.Candidatos.Remove(candidato);
        await _contexto.SaveChangesAsync(cancellationToken);

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
