using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RecruitAI.Datos.Persistencia;

public class CherokeeDbContextLecturaFactory : IDbContextFactory<CherokeeDbContext>
{
    private readonly DbContextOptions<CherokeeDbContext> _opcionesLectura;

    public CherokeeDbContextLecturaFactory(DbContextOptions<CherokeeDbContext> opcionesLectura)
    {
        _opcionesLectura = opcionesLectura ?? throw new ArgumentNullException(nameof(opcionesLectura));
    }

    public CherokeeDbContext CreateDbContext()
    {
        return new CherokeeDbContext(_opcionesLectura);
    }

    public ValueTask<CherokeeDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(CreateDbContext());
    }
}
