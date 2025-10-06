namespace RecruitAI.Contratos.Constantes;

public static class RolesAplicacion
{
    public const string Administrador = "Administrador";
    public const string Reclutador = "Reclutador";

    public static string[] Todos => new[] { Administrador, Reclutador };
    public static string RolesLectura => string.Join(',', Todos);
}
