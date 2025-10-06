using System.Text.Json;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RecruitAI.Contratos.Interfaces.Repositorios;
using RecruitAI.Contratos.Interfaces.Servicios;
using RecruitAI.Datos.Entidades;
using RecruitAI.Datos.Persistencia;
using RecruitAI.Datos.Repositorios;
using RecruitAI.Servicios.Implementaciones;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((contexto, servicios, configuracion) =>
    configuracion.ReadFrom.Configuration(contexto.Configuration).ReadFrom.Services(servicios));

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RecruitAI API",
        Version = "v1",
        Description = "API de reclutamiento asistido por IA"
    });

    var esquema = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce un token JWT válido"
    };

    opciones.AddSecurityDefinition("Bearer", esquema);
    opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            esquema,
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("cors-desarrollo", configuracion =>
    {
        configuracion
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

var cadenaCompleta = builder.Configuration.GetConnectionString("RecruitAIConexionCompleta")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'RecruitAIConexionCompleta'.");
var cadenaLectura = builder.Configuration.GetConnectionString("RecruitAIConexionSoloLectura")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'RecruitAIConexionSoloLectura'.");

builder.Services.AddDbContext<CherokeeDbContext>(opciones =>
{
    opciones.UseSqlServer(cadenaCompleta);
});

builder.Services.AddDbContext<CherokeeDbContextLectura>(opciones =>
{
    opciones.UseSqlServer(cadenaLectura);
});

builder.Services.AddScoped<IPuestoRepositorio, PuestoRepositorio>();
builder.Services.AddScoped<ICandidatoRepositorio, CandidatoRepositorio>();
builder.Services.AddScoped<ICoincidenciasServicio, CoincidenciasServicio>();
builder.Services.AddScoped<IIaServicio, IaServicio>();
builder.Services.AddHttpClient<IEmbeddingsServicio, EmbeddingsServicio>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseCors("cors-desarrollo");

app.UseHttpsRedirection();

// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await InicializarDatosAsync(app.Services);

app.Run();

static async Task InicializarDatosAsync(IServiceProvider servicios)
{
    using var alcance = servicios.CreateScope();
    var contexto = alcance.ServiceProvider.GetRequiredService<CherokeeDbContext>();

    await contexto.Database.EnsureCreatedAsync();

    if (!await contexto.Puestos.AnyAsync())
    {
        var puestoDemo = new Puesto
        {
            Id = Guid.NewGuid(),
            Titulo = "Desarrollador .NET",
            Descripcion = "Buscamos un desarrollador .NET con pasión por crear APIs escalables y mantenibles.",
            Seniority = "Semi Senior",
            Ubicacion = "Remoto",
            HabilidadesRequeridasJson = JsonSerializer.Serialize(new[] { ".NET", "SQL", "Azure" })
        };

        contexto.Puestos.Add(puestoDemo);

        var candidato1 = new Candidato
        {
            Id = Guid.NewGuid(),
            NombreCompleto = "María González",
            Email = "maria@example.com",
            Fuente = "LinkedIn",
            CvTexto = "Desarrolladora con experiencia en .NET, Azure y SQL Server.",
        };

        var candidato2 = new Candidato
        {
            Id = Guid.NewGuid(),
            NombreCompleto = "Carlos Pérez",
            Email = "carlos@example.com",
            Fuente = "Recomendación",
            CvTexto = "Ingeniero de software con foco en microservicios y bases de datos relacionales.",
        };

        contexto.Candidatos.AddRange(candidato1, candidato2);

        await contexto.SaveChangesAsync();
    }
}
