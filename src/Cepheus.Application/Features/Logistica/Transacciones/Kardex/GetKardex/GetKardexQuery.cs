using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Kardex.GetKardex;

/// <summary>
/// Kardex (Movimiento de Artículos) de un artículo en una planta, entre dos fechas. Solo lectura.
/// </summary>
public sealed record GetKardexQuery(
    string PlantaCode,
    string ArticuloCode,
    DateTime FechaDesde,
    DateTime FechaHasta) : IRequest<KardexResponse>;
