using RecruitAI.Web.Configuracion;

var builder = WebApplication.CreateBuilder(args)
    .RegistrarServiciosRecruitAI();

var app = builder.Build();

app.ConfigurarAplicacionRecruitAI();

await app.InicializarDatosAsync();

app.Run();
