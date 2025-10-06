using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitAI.Contratos.Constantes;
using RecruitAI.Contratos.Dtos.Puestos;
using RecruitAI.Datos.Entidades;
using RecruitAI.Datos.Persistencia;

namespace RecruitAI.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PuestosController : ControllerBase
{
    private readonly CherokeeDbContext _contextoEscritura;
    private readonly IDbContextFactory<CherokeeDbContext> _contextoLecturaFactory;

    public PuestosController(
        CherokeeDbContext contextoEscritura,
        IDbContextFactory<CherokeeDbContext> contextoLecturaFactory)
    {
        _contextoEscritura = contextoEscritura;
        _contextoLecturaFactory = contextoLecturaFactory;
    }

    [Authorize(Policy = "lectura")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PuestoDto>>> ObtenerAsync(CancellationToken cancellationToken)
    {
        await using var contextoLectura = await _contextoLecturaFactory.CreateDbContextAsync(cancellationToken);
        var puestos = await contextoLectura.Puestos
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var resultado = puestos.Select(MapearPuesto).ToList();
        return Ok(resultado);
    }

    [Authorize(Policy = "lectura")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PuestoDto>> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var contextoLectura = await _contextoLecturaFactory.CreateDbContextAsync(cancellationToken);
        var puesto = await contextoLectura.Puestos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (puesto is null)
        {
            return NotFound();
        }

        return Ok(MapearPuesto(puesto));
    }

    [Authorize(Policy = "administracion")]
    [HttpPost]
    public async Task<ActionResult<PuestoDto>> CrearAsync([FromBody] CrearPuestoDto dto, CancellationToken cancellationToken)
    {
        var puesto = new Puesto
        {
            Id = Guid.NewGuid(),
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            Seniority = dto.Seniority,
            Ubicacion = dto.Ubicacion,
            HabilidadesRequeridasJson = dto.HabilidadesRequeridas.Any()
                ? JsonSerializer.Serialize(dto.HabilidadesRequeridas)
                : null,
            CreadoEl = DateTime.UtcNow
        };

        await _contextoEscritura.Puestos.AddAsync(puesto, cancellationToken);
        await _contextoEscritura.SaveChangesAsync(cancellationToken);

        var resultado = MapearPuesto(puesto);
        return CreatedAtAction(nameof(ObtenerPorIdAsync), new { id = puesto.Id }, resultado);
    }

    [Authorize(Policy = "administracion")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PuestoDto>> ActualizarAsync(Guid id, [FromBody] EditarPuestoDto dto, CancellationToken cancellationToken)
    {
        var puesto = await _contextoEscritura.Puestos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (puesto is null)
        {
            return NotFound();
        }

        puesto.Titulo = dto.Titulo;
        puesto.Descripcion = dto.Descripcion;
        puesto.Seniority = dto.Seniority;
        puesto.Ubicacion = dto.Ubicacion;
        puesto.HabilidadesRequeridasJson = dto.HabilidadesRequeridas.Any()
            ? JsonSerializer.Serialize(dto.HabilidadesRequeridas)
            : null;

        await _contextoEscritura.SaveChangesAsync(cancellationToken);

        return Ok(MapearPuesto(puesto));
    }

    [Authorize(Policy = "administracion")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> EliminarAsync(Guid id, CancellationToken cancellationToken)
    {
        var puesto = await _contextoEscritura.Puestos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (puesto is null)
        {
            return NotFound();
        }

        _contextoEscritura.Puestos.Remove(puesto);
        await _contextoEscritura.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static PuestoDto MapearPuesto(Puesto puesto)
    {
        var habilidades = puesto.HabilidadesRequeridasJson is null
            ? Array.Empty<string>()
            : JsonSerializer.Deserialize<string[]>(puesto.HabilidadesRequeridasJson) ?? Array.Empty<string>();

        return new PuestoDto
        {
            Id = puesto.Id,
            Titulo = puesto.Titulo,
            Descripcion = puesto.Descripcion,
            Seniority = puesto.Seniority,
            Ubicacion = puesto.Ubicacion,
            HabilidadesRequeridas = habilidades,
            CreadoEl = puesto.CreadoEl
        };
    }
}
