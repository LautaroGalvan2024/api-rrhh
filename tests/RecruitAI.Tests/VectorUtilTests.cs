using FluentAssertions;
using RecruitAI.Servicios.Utilidades;
using Xunit;

namespace RecruitAI.Tests;

public class VectorUtilTests
{
    [Fact]
    public void CalcularCoseno_DeberiaRetornarUnoParaVectoresIguales()
    {
        var vector = new[] { 1f, 2f, 3f };

        var resultado = VectorUtil.CalcularCoseno(vector, vector);

        resultado.Should().BeApproximately(1d, 1e-6);
    }
}
