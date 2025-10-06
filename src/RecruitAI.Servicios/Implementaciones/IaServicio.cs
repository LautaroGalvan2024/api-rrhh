using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using RecruitAI.Contratos.Dtos.Ia;
using RecruitAI.Contratos.Interfaces.Servicios;

namespace RecruitAI.Servicios.Implementaciones;

public class IaServicio : IIaServicio
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public IaServicio(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> ExtraerCvAsync(ExtraerCvRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.CvTexto))
        {
            throw new ArgumentException("El texto del CV no puede estar vacío.", nameof(request));
        }

        var prompt = $"""
Extrae la información clave del siguiente CV en formato JSON válido con las propiedades: nombreCompleto, email, experiencia (lista de objetos con titulo, empresa, descripcion), habilidades (arreglo de cadenas) y educacion (lista de objetos con institucion, titulo, periodo). Devuelve únicamente el JSON sin comentarios ni texto adicional.

CV:
{request.CvTexto}
""";

        return await EjecutarChatAsync(prompt, 0.1, cancellationToken);
    }

    public async Task<double> PuntuarAsync(PuntuarRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.CvTexto))
        {
            throw new ArgumentException("El texto del CV no puede estar vacío.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.DescripcionPuesto))
        {
            throw new ArgumentException("La descripción del puesto no puede estar vacía.", nameof(request));
        }

        var prompt = $"""
Analiza la siguiente descripción de puesto y el CV. Devuelve únicamente un número entre 0 y 100 que represente el grado de ajuste del candidato al puesto. Utiliza decimales si es necesario. No incluyas explicación adicional, solo el número.

Puesto:
{request.DescripcionPuesto}

CV:
{request.CvTexto}
""";

        var respuesta = await EjecutarChatAsync(prompt, 0.0, cancellationToken);

        var coincidencia = Regex.Match(respuesta, "-?\\d+(?:[\\.,]\\d+)?");
        if (!coincidencia.Success)
        {
            throw new InvalidOperationException("No se pudo interpretar el puntaje devuelto por OpenAI.");
        }

        var puntaje = double.Parse(coincidencia.Value.Replace(',', '.'), CultureInfo.InvariantCulture);
        return Math.Clamp(puntaje, 0d, 100d);
    }

    private async Task<string> EjecutarChatAsync(string promptUsuario, double? temperatura, CancellationToken cancellationToken)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("No se configuró la clave de OpenAI. Actualiza appsettings.json o las variables de entorno.");
        }

        var modelo = _configuration["OpenAI:ChatModel"] ?? "gpt-4o-mini";
        var temperaturaPredeterminada = _configuration.GetValue<double?>("OpenAI:ChatTemperature") ?? 0.1;

        _httpClient.BaseAddress ??= new Uri("https://api.openai.com/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var solicitud = new
        {
            model = modelo,
            temperature = temperatura ?? temperaturaPredeterminada,
            messages = new object[]
            {
                new { role = "system", content = "Eres un asistente experto en reclutamiento que responde de forma precisa y concisa." },
                new { role = "user", content = promptUsuario }
            }
        };

        using var respuesta = await _httpClient.PostAsJsonAsync("v1/chat/completions", solicitud, cancellationToken);
        respuesta.EnsureSuccessStatusCode();

        var contenido = await respuesta.Content.ReadFromJsonAsync<ChatCompletionRespuesta>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("No se pudo deserializar la respuesta de OpenAI.");

        var mensaje = contenido.Choices.FirstOrDefault()?.Message?.Content;
        if (string.IsNullOrWhiteSpace(mensaje))
        {
            throw new InvalidOperationException("OpenAI no devolvió contenido en la respuesta.");
        }

        return mensaje.Trim();
    }

    private sealed record ChatCompletionRespuesta([
        property: JsonPropertyName("choices")
    ] List<ChatChoice> Choices);

    private sealed record ChatChoice([
        property: JsonPropertyName("message")
    ] ChatMessage? Message);

    private sealed record ChatMessage([
        property: JsonPropertyName("role")
    ] string Role, [
        property: JsonPropertyName("content")
    ] string Content);
}
