using System.Text;
using System.Text.Json;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RecruitAI.Contratos.Configuracion;
using RecruitAI.Contratos.Constantes;
using RecruitAI.Contratos.Interfaces.Repositorios;
using RecruitAI.Contratos.Interfaces.Servicios;
using RecruitAI.Datos.Entidades;
using RecruitAI.Datos.Persistencia;
using RecruitAI.Datos.Repositorios;
using RecruitAI.Servicios.Implementaciones;
using Serilog;

namespace RecruitAI.Web.Configuracion;

public static class AddServices
{
    public static WebApplicationBuilder RegistrarServiciosRecruitAI(this WebApplicationBuilder builder)
    {
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
        var opcionesLectura = new DbContextOptionsBuilder<CherokeeDbContext>()
            .UseSqlServer(cadenaLectura)
            .Options;

        builder.Services.AddSingleton<IDbContextFactory<CherokeeDbContext>>(
            _ => new CherokeeDbContextLecturaFactory(opcionesLectura));

        var jwtSection = builder.Configuration.GetSection("Jwt");
        var jwtOptions = jwtSection.Get<JwtOptions>()
            ?? throw new InvalidOperationException("No se encontró la configuración de Jwt.");
        if (string.IsNullOrWhiteSpace(jwtOptions.Secreto))
        {
            throw new InvalidOperationException("El secreto para firmar JWT no está configurado.");
        }

        builder.Services.Configure<JwtOptions>(jwtSection);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opciones =>
            {
                opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtOptions.Emisor,
                    ValidAudience = jwtOptions.Audiencia,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secreto))
                };
            });

        builder.Services.AddAuthorization(opciones =>
        {
            opciones.AddPolicy("lectura", politica => politica.RequireRole(RolesAplicacion.Todos));
            opciones.AddPolicy("administracion", politica => politica.RequireRole(RolesAplicacion.Administrador));
        });

        builder.Services.AddScoped<IPuestoRepositorio, PuestoRepositorio>();
        builder.Services.AddScoped<ICandidatoRepositorio, CandidatoRepositorio>();
        builder.Services.AddScoped<ICoincidenciasServicio, CoincidenciasServicio>();
        builder.Services.AddScoped<IAuthServicio, AuthServicio>();
        builder.Services.AddHttpClient<IIaServicio, IaServicio>();
        builder.Services.AddHttpClient<IEmbeddingsServicio, EmbeddingsServicio>();

        return builder;
    }

    public static WebApplication ConfigurarAplicacionRecruitAI(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();

        app.UseCors("cors-desarrollo");

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }

    public static async Task InicializarDatosAsync(this WebApplication app)
    {
        using var alcance = app.Services.CreateScope();
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
}
