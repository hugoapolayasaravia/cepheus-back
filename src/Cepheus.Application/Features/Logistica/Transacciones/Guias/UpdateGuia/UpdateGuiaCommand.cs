// Cepheus.Application/Features/Logistica/Transacciones/Guias/UpdateGuia/UpdateGuiaCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.UpdateGuia
{
    /// <summary>
    /// Edita la cabecera. No permite cambiar Planta ni Code (son la clave).
    /// Las líneas se gestionan con los commands de detalle.
    /// </summary>
    public record UpdateGuiaCommand(
        string PlantaCode,
        string Code,
        DateTime FechaEmision,
        string Hora,
        string ProveedorCode,
        string MotivoCode,
        string Direccion,
        string PuntoPartida,
        string TransportistaCode,
        string ConductorCode,
        string VehiculoCode,
        string? Observaciones,
        byte[] RowVersion
    ) : IRequest<GuiaResponse>;
}
