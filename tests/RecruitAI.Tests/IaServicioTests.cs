using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using RecruitAI.Contratos.Dtos.Ia;
using RecruitAI.Servicios.Implementaciones;
using Xunit;

namespace RecruitAI.Tests;

public class IaServicioTests
{
    [Fact]
    public async Task ExtraerCvAsync_DevuelveContenidoNormalizado()
    {
        var respuesta = "{\"choices\":[{\"message\":{\"content\":\"{\\\"nombreCompleto\\\":\\\"Jane Doe\\\"}\"}}]}";
        var httpClient = CrearHttpClient(respuesta);
        var configuracion = CrearConfiguracion();
        var servicio = new IaServicio(httpClient, configuracion);

        var resultado = await servicio.ExtraerCvAsync(new ExtraerCvRequest { CvTexto = "Experiencia en .NET" });

        Assert.Equal("{\"nombreCompleto\":\"Jane Doe\"}", resultado);
        Assert.Equal("https://api.openai.com/", httpClient.BaseAddress?.ToString());
        Assert.Equal(new AuthenticationHeaderValue("Bearer", "test-key"), httpClient.DefaultRequestHeaders.Authorization);
    }

    [Fact]
    public async Task PuntuarAsync_InterpretaValorNumerico()
    {
        var respuesta = "{\"choices\":[{\"message\":{\"content\":\"82.75\"}}]}";
        var httpClient = CrearHttpClient(respuesta);
        var configuracion = CrearConfiguracion();
        var servicio = new IaServicio(httpClient, configuracion);

        var puntaje = await servicio.PuntuarAsync(new PuntuarRequest
        {
            CvTexto = "Experiencia en Azure y .NET",
            DescripcionPuesto = "Buscamos experiencia en .NET"
        });

        Assert.Equal(82.75, puntaje);
    }

    private static HttpClient CrearHttpClient(string cuerpoRespuesta)
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(cuerpoRespuesta, Encoding.UTF8, "application/json")
        });
        return new HttpClient(handler);
    }

    private static IConfiguration CrearConfiguracion()
    {
        var valores = new Dictionary<string, string?>
        {
            ["OpenAI:ApiKey"] = "test-key",
            ["OpenAI:ChatModel"] = "gpt-test"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(valores)
            .Build();
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _respuesta;

        public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respuesta)
        {
            _respuesta = respuesta;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_respuesta(request));
    }
}
