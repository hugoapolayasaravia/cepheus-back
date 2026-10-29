namespace Cepheus.Application.Features.Comunes.PeriodosIgv.Common;

/// <summary>Lógica pura de rangos de fechas de los períodos de IGV (sin base de datos).</summary>
public static class PeriodoIgvCalculo
{
    /// <summary>True si la fecha cae dentro del período (ambos extremos inclusive; fin nulo = abierto).</summary>
    public static bool Cubre(DateOnly inicio, DateOnly? fin, DateOnly fecha)
        => inicio <= fecha && (fin is null || fin.Value >= fecha);

    /// <summary>True si dos períodos comparten al menos un día.</summary>
    public static bool Solapan(DateOnly inicioA, DateOnly? finA, DateOnly inicioB, DateOnly? finB)
        => (finB is null || inicioA <= finB.Value) && (finA is null || inicioB <= finA.Value);
}
