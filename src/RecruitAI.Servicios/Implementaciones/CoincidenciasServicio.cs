using System.Linq;
using RecruitAI.Contratos.Dtos.Coincidencias;
using RecruitAI.Contratos.Interfaces.Repositorios;
using RecruitAI.Contratos.Interfaces.Servicios;
using RecruitAI.Datos.Entidades;
using RecruitAI.Servicios.Utilidades;

namespace RecruitAI.Servicios.Implementaciones;

public class CoincidenciasServicio : ICoincidenciasServicio
{
    private readonly IPuestoRepositorio _puestoRepositorio;
    private readonly ICandidatoRepositorio _candidatoRepositorio;
    private readonly IEmbeddingsServicio _embeddingsServicio;

    public CoincidenciasServicio(
        IPuestoRepositorio puestoRepositorio,
        ICandidatoRepositorio candidatoRepositorio,
        IEmbeddingsServicio embeddingsServicio)
    {
        _puestoRepositorio = puestoRepositorio;
        _candidatoRepositorio = candidatoRepositorio;
        _embeddingsServicio = embeddingsServicio;
    }

    public async Task<IReadOnlyCollection<CoincidenciaDto>> TopPorPuestoAsync(Guid puestoId, int cantidad = 20, double? umbral = null, CancellationToken cancellationToken = default)
    {
        var puesto = await _puestoRepositorio.ObtenerConEmbeddingAsync(puestoId, cancellationToken)
            ?? throw new InvalidOperationException("No se encontró el puesto solicitado.");

        if (puesto is not Puesto puestoEntidad || puestoEntidad.EmbeddingPuesto?.Vector is null)
        {
            throw new InvalidOperationException("El puesto no tiene un embedding generado. Genera el vector antes de calcular coincidencias.");
        }

        var vectorPuesto = _embeddingsServicio.ConvertirAFlotantes(puestoEntidad.EmbeddingPuesto.Vector);

        var candidatos = await _candidatoRepositorio.ListarConEmbeddingAsync(cancellationToken);

        var coincidencias = new List<CoincidenciaDto>();
        foreach (var candidato in candidatos)
        {
            if (candidato is not Candidato candidatoEntidad)
            {
                continue;
            }

            var vectorBytes = candidatoEntidad.EmbeddingCandidato?.Vector;
            if (vectorBytes is null)
            {
                continue;
            }

            var vectorCandidato = _embeddingsServicio.ConvertirAFlotantes(vectorBytes);
            var similitud = VectorUtil.CalcularCoseno(vectorPuesto, vectorCandidato);
            var puntaje = Math.Round(similitud * 100, 2, MidpointRounding.AwayFromZero);

            if (umbral.HasValue && similitud < umbral.Value)
            {
                continue;
            }

            coincidencias.Add(new CoincidenciaDto
            {
                CandidatoId = candidatoEntidad.Id,
                NombreCompleto = candidatoEntidad.NombreCompleto,
                Email = candidatoEntidad.Email,
                Puntaje = puntaje
            });
        }

        return coincidencias
            .OrderByDescending(x => x.Puntaje)
            .Take(cantidad)
            .ToList();
    }
}
