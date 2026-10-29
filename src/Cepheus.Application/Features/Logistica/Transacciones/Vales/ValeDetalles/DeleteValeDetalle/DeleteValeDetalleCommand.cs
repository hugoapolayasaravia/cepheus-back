using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.ValeDetalles.DeleteValeDetalle;

/// <summary>
/// Elimina una línea de un vale Pendiente y renumera los ítems (Logi_sp_Actualiza_Item_Vales).
/// Si la línea venía de un material de la OT, ese material vuelve a pendiente (01).
/// </summary>
public sealed record DeleteValeDetalleCommand(
    string PlantaCode,
    string ValeCode,
    string ArticuloCode) : IRequest<ValeResponse>;
