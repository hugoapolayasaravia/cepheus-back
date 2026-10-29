using Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.UpdateOTResponsable
{
    /// <summary>
    /// Solo permite corregir TiempoProceso/Basico/CostoTotal — la clave
    /// (planta, OT, fecha, trabajador) no se edita: si cambió el trabajador
    /// o la fecha, es un registro distinto (se borra el anterior y se crea
    /// uno nuevo).
    /// </summary>
    public record UpdateOTResponsableCommand(
        string PlantaCode,
        string OrdenTrabajoCode,
        DateTime FechaProceso,
        string TrabajadorCode,
        decimal TiempoProceso,
        decimal Basico,
        decimal CostoTotal,
        byte[] RowVersion
    ) : IRequest<OTResponsableResponse>;
}
