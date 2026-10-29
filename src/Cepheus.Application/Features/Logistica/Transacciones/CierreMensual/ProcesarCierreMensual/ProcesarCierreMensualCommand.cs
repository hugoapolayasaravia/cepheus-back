using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.ProcesarCierreMensual;

/// <summary>
/// Procesa el cierre mensual de artículos de una planta.
/// PeriodoCode (AAAAMM) solo es obligatorio en el PRIMER cierre de la planta (período de apertura explícito);
/// en los siguientes se cierra siempre el período que sigue al último cerrado.
/// </summary>
public sealed record ProcesarCierreMensualCommand(string PlantaCode, string? PeriodoCode)
    : IRequest<CierreMensualResponse>;

public sealed record CierreMensualResponse(
    string PlantaCode,
    string PeriodCode,
    DateTime FechaCierre,
    int ArticulosConSaldo,
    int ArticulosConMovimientos,
    decimal ImporteTotal,
    int DocumentosRevalorizados,
    int Incidencias,
    decimal DiferenciaConStock,
    // Artículos cuyo costo promedio vivo se alineó con el del cierre.
    int CostosPromedioAlineados);
