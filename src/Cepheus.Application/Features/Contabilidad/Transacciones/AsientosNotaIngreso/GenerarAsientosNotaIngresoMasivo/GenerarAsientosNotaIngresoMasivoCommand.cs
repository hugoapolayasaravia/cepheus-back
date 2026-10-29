using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.GenerarAsientosNotaIngresoMasivo
{
    /// <summary>
    /// Genera el asiento de todas las notas de ingreso Procesadas de la planta, con fecha de recepción en el rango,
    /// que todavía no tienen asiento. Cada nota se procesa en su propia transacción: si una falla, las demás continúan
    /// (el legacy preguntaba "continuamos con el siguiente documento").
    /// </summary>
    public record GenerarAsientosNotaIngresoMasivoCommand(string PlantaCode, DateTime FechaDesde, DateTime FechaHasta)
        : IRequest<GenerarAsientosMasivoResponse>;
}
