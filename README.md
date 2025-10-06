# RecruitAI

Solución .NET 8 por capas para gestionar procesos de reclutamiento con soporte para embeddings de OpenAI y coincidencias basadas en similitud de vectores.

## Estructura

```
RecruitAI/
├─ RecruitAI.sln
├─ src/
│  ├─ RecruitAI.Web/
│  ├─ RecruitAI.Contratos/
│  ├─ RecruitAI.Datos/
│  └─ RecruitAI.Servicios/
└─ tests/
   └─ RecruitAI.Tests/
```

## Prerrequisitos

* .NET SDK 8.0
* SQL Server (local o remoto) accesible mediante las cadenas de conexión configuradas.

## Configuración

Actualiza las cadenas de conexión y secretos en `src/RecruitAI.Web/appsettings.json` o mediante variables de entorno:

```json
{
  "ConnectionStrings": {
    "RecruitAIConexionCompleta": "Server=DESKTOP-8L1PQ7F;Database=RecruitAI;Trusted_Connection=True;TrustServerCertificate=True;",
    "RecruitAIConexionSoloLectura": "Server=DESKTOP-8L1PQ7F;Database=RecruitAI;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "OpenAI": {
    "ApiKey": "sk-xxxx"
  },
  "Jwt": {
    "Emisor": "http://localhost",
    "Audiencia": "http://localhost",
    "Secreto": "cambialo-porfavor"
  }
}
```

Variables recomendadas:

* `ConnectionStrings__RecruitAIConexionCompleta`
* `ConnectionStrings__RecruitAIConexionSoloLectura`
* `OpenAI__ApiKey`
* `Jwt__Emisor`
* `Jwt__Audiencia`
* `Jwt__Secreto`

## Comandos útiles

```bash
# Compilar la solución completa
dotnet build

# Crear una migración inicial
dotnet ef migrations add Init --project src/RecruitAI.Datos --startup-project src/RecruitAI.Web

# Aplicar las migraciones
dotnet ef database update --project src/RecruitAI.Datos --startup-project src/RecruitAI.Web

# Ejecutar la API
dotnet run --project src/RecruitAI.Web
```

## Endpoints principales

* `GET /api/puestos` — Lista de puestos utilizando la base de solo lectura.
* `GET /api/candidatos` — Lista de candidatos utilizando la base de solo lectura.
* `POST /api/coincidencias/puestos/{puestoId}/embedding` — Genera y persiste el embedding del puesto.
* `POST /api/coincidencias/candidatos/{candidatoId}/embedding` — Genera y persiste el embedding del candidato.
* `GET /api/coincidencias/puestos/{puestoId}/top?cantidad=20&umbral=0.65` — Obtiene coincidencias ordenadas descendentemente por puntaje.

Swagger incluye soporte para autenticación JWT (pendiente de habilitar). FluentValidation valida automáticamente los DTOs de entrada.

## Siguientes pasos

* Implementar el servicio de IA para extracción y puntuación real usando OpenAI.
* Completar flujo de autenticación JWT y autorización por roles.
* Agregar más pruebas unitarias e integración.
* Automatizar despliegue continuo y parametrizar las conexiones de lectura y escritura.
