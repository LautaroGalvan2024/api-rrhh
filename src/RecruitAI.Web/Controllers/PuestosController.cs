using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitAI.Contratos.Dtos.Puestos;
using RecruitAI.Datos.Entidades;
using RecruitAI.Datos.Persistencia;

namespace RecruitAI.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PuestosController : ControllerBase
{
    private readonly CherokeeDbContext _contexto;

    public PuestosController(CherokeeDbContext contexto)
    {
        _contexto = contexto;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PuestoDto>>> ObtenerAsync(CancellationToken cancellationToken)
    {
        var puestos = await _contexto.Puestos
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var resultado = puestos.Select(MapearPuesto).ToList();
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PuestoDto>> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var puesto = await _contexto.Puestos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (puesto is null)
        {
            return NotFound();
        }

        return Ok(MapearPuesto(puesto));
    }

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

        await _contexto.Puestos.AddAsync(puesto, cancellationToken);
        await _contexto.SaveChangesAsync(cancellationToken);

        var resultado = MapearPuesto(puesto);
        return CreatedAtAction(nameof(ObtenerPorIdAsync), new { id = puesto.Id }, resultado);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PuestoDto>> ActualizarAsync(Guid id, [FromBody] EditarPuestoDto dto, CancellationToken cancellationToken)
    {
        var puesto = await _contexto.Puestos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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

        await _contexto.SaveChangesAsync(cancellationToken);

        return Ok(MapearPuesto(puesto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> EliminarAsync(Guid id, CancellationToken cancellationToken)
    {
        var puesto = await _contexto.Puestos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (puesto is null)
        {
            return NotFound();
        }

        _contexto.Puestos.Remove(puesto);
        await _contexto.SaveChangesAsync(cancellationToken);

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
