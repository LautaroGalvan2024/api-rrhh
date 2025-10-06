using Microsoft.EntityFrameworkCore;

namespace RecruitAI.Datos.Persistencia;

public class CherokeeDbContextLectura : CherokeeDbContext
{
    public CherokeeDbContextLectura(DbContextOptions<CherokeeDbContextLectura> options)
        : base(options)
    {
    }
}
