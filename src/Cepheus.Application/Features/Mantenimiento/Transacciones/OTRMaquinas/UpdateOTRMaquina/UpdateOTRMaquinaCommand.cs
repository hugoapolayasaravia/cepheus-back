using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.UpdateOTRMaquina
{
    /// <summary>
    /// Solo permite corregir FechaProceso/Cantidad/Horas — la clave
    /// (planta, OT, máquina) no se edita.
    /// </summary>
    public record UpdateOTRMaquinaCommand(
        string PlantaCode,
        string OrdenTrabajoCode,
        string MaquinaCode,
        DateTime FechaProceso,
        decimal Cantidad,
        decimal Horas,
        byte[] RowVersion
    ) : IRequest<OTRMaquinaResponse>;
}
