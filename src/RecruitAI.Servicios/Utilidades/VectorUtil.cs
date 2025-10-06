using System.Runtime.InteropServices;

namespace RecruitAI.Servicios.Utilidades;

public static class VectorUtil
{
    public static double CalcularCoseno(IReadOnlyList<float> vectorA, IReadOnlyList<float> vectorB)
    {
        if (vectorA.Count == 0 || vectorB.Count == 0)
        {
            return 0d;
        }

        if (vectorA.Count != vectorB.Count)
        {
            throw new ArgumentException("Los vectores deben tener la misma longitud.");
        }

        double producto = 0d;
        double normaA = 0d;
        double normaB = 0d;

        for (var i = 0; i < vectorA.Count; i++)
        {
            var a = vectorA[i];
            var b = vectorB[i];
            producto += a * b;
            normaA += a * a;
            normaB += b * b;
        }

        if (normaA == 0 || normaB == 0)
        {
            return 0d;
        }

        return producto / (Math.Sqrt(normaA) * Math.Sqrt(normaB));
    }

    public static byte[] ConvertirABytes(float[] valores)
    {
        var resultado = new byte[valores.Length * sizeof(float)];
        var destino = MemoryMarshal.Cast<byte, float>(resultado.AsSpan());
        valores.AsSpan().CopyTo(destino);
        return resultado;
    }

    public static float[] ConvertirAFlotantes(byte[] datos)
    {
        if (datos.Length % sizeof(float) != 0)
        {
            throw new ArgumentException("El arreglo de bytes no representa una secuencia válida de flotantes.");
        }

        var resultado = new float[datos.Length / sizeof(float)];
        var origen = MemoryMarshal.Cast<byte, float>(datos.AsSpan());
        origen.CopyTo(resultado);
        return resultado;
    }
}
