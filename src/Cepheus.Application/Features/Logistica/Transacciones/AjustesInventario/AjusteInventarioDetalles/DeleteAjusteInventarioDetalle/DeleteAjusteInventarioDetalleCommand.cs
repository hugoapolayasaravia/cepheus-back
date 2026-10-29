using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.AjusteInventarioDetalles.DeleteAjusteInventarioDetalle;

/// <summary>Elimina una línea pendiente y renumera los ítems (Logi_sp_Actualiza_Item_Ajustes).</summary>
public sealed record DeleteAjusteInventarioDetalleCommand(
    string PlantaCode,
    string AjusteCode,
    string ArticuloCode) : IRequest<AjusteInventarioResponse>;
