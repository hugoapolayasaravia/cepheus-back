using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Transferencias.GetTransferenciasPendientes;

/// <summary>
/// Vales de transferencia (tipo 005) enviados a una planta y todavía no recibidos (el dw_4 del botón
/// "Transferencias" del Maestro de Nota de Ingresos).
/// </summary>
public sealed record GetTransferenciasPendientesQuery(string PlantaDestinoCode)
    : IRequest<IReadOnlyList<TransferenciaPendienteResponse>>;

public sealed record TransferenciaPendienteLineaResponse(
    int ItemNumber,
    string ArticuloCode,
    string ArticuloName,
    string UnidadMedidaCode,
    decimal Cantidad,
    decimal Precio,
    decimal Total);

public sealed record TransferenciaPendienteResponse(
    string PlantaOrigenCode,
    string PlantaOrigenName,
    string ValeCode,
    DateTime FechaEntrega,
    decimal Total,
    IReadOnlyList<TransferenciaPendienteLineaResponse> Lineas);
