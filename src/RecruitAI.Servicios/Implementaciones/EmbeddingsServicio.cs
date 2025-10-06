using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using RecruitAI.Contratos.Interfaces.Servicios;
using RecruitAI.Servicios.Utilidades;

namespace RecruitAI.Servicios.Implementaciones;

public class EmbeddingsServicio : IEmbeddingsServicio
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public EmbeddingsServicio(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<float[]> GenerarEmbeddingAsync(string texto, string modelo = "text-embedding-3-large", CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("No se configuró la clave de OpenAI. Actualiza appsettings.json o las variables de entorno.");
        }

        _httpClient.BaseAddress ??= new Uri("https://api.openai.com/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var request = new
        {
            model = modelo,
            input = texto
        };

        using var respuesta = await _httpClient.PostAsJsonAsync("v1/embeddings", request, cancellationToken);
        respuesta.EnsureSuccessStatusCode();

        var contenido = await respuesta.Content.ReadFromJsonAsync<EmbeddingRespuesta>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("No se pudo deserializar la respuesta de OpenAI.");

        var vector = contenido.data.FirstOrDefault()?.embedding
            ?? throw new InvalidOperationException("OpenAI no devolvió ningún vector de embedding.");

        return vector.ToArray();
    }

    public byte[] ConvertirABytes(float[] vector) => VectorUtil.ConvertirABytes(vector);

    public float[] ConvertirAFlotantes(byte[] datos) => VectorUtil.ConvertirAFlotantes(datos);

    private sealed record EmbeddingRespuesta(List<EmbeddingDato> data);

    private sealed record EmbeddingDato(List<float> embedding);
}
