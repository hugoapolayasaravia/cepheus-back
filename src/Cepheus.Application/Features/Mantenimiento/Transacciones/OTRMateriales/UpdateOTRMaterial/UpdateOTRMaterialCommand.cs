using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.UpdateOTRMaterial
{
    /// <summary>
    /// Solo permite corregir Cantidad/CostoUnitario/CostoTotal/EstadoCode —
    /// la clave (planta, OT, fecha, artículo) no se edita.
    /// </summary>
    public record UpdateOTRMaterialCommand(
        string PlantaCode,
        string OrdenTrabajoCode,
        DateTime FechaProceso,
        string ArticuloCode,
        decimal Cantidad,
        decimal CostoUnitario,
        decimal CostoTotal,
        string? EstadoCode,
        byte[] RowVersion
    ) : IRequest<OTRMaterialResponse>;
}
