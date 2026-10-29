using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.GetCierreMensualEstado;

/// <summary>
/// Datos de la pantalla de cierre (equivale al mensaje del PowerBuilder: mes de cierre anterior / mes a procesar).
/// </summary>
public sealed record GetCierreMensualEstadoQuery(string PlantaCode) : IRequest<CierreMensualEstadoResponse>;

public sealed record CierreMensualEstadoResponse(
    string PlantaCode,
    string PlantaName,
    /// <summary>Último período cerrado (AAAAMM); null si la planta nunca cerró.</summary>
    string? UltimoPeriodoCerrado,
    DateTime? FechaUltimoCierre,
    /// <summary>Siguiente período a procesar; null en el primer cierre (hay que indicarlo).</summary>
    string? ProximoPeriodo,
    /// <summary>En el primer cierre: período del primer movimiento registrado, como sugerencia.</summary>
    string? PeriodoSugerido,
    bool RequierePeriodoApertura,
    bool PuedeProcesar,
    /// <summary>Si no se puede procesar, la razón.</summary>
    string? Motivo,
    int IncidenciasUltimoCierre,
    decimal DiferenciaUltimoCierre);
