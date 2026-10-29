using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.GenerarAperturaKardex;

/// <summary>
/// Genera la apertura del Kardex de una planta: un movimiento de Apertura por cada artículo con stock que el Kardex
/// todavía no explica. Se ejecuta una vez, antes del primer cierre (reemplaza la carga de "stock iniciales" del legacy).
/// FechaApertura es opcional: por defecto la fecha del primer movimiento del Kardex de la planta, o hoy si no hay.
/// </summary>
public sealed record GenerarAperturaKardexCommand(string PlantaCode, DateTime? FechaApertura)
    : IRequest<AperturaKardexResponse>;

public sealed record AperturaKardexResponse(
    string PlantaCode,
    DateTime FechaApertura,
    int ArticulosConApertura,
    int ArticulosQueYaTeniaApertura,
    int ArticulosSinDiferencia,
    // Artículos donde el Kardex ya suma más que el stock actual (no se les genera apertura): revisar.
    int ArticulosInconsistentes,
    IReadOnlyList<string> CodigosInconsistentes);
