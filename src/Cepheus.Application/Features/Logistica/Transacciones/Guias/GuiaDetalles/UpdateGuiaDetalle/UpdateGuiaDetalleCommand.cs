// Cepheus.Application/Features/Logistica/Transacciones/Guias/GuiaDetalles/UpdateGuiaDetalle/UpdateGuiaDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.GuiaDetalles.UpdateGuiaDetalle
{
    /// <summary>
    /// Modifica cantidad y verificación de una línea. El artículo NO se puede
    /// cambiar (en el legacy el código de artículo queda bloqueado al
    /// modificar); para otro artículo se elimina la línea y se agrega una nueva.
    /// </summary>
    public record UpdateGuiaDetalleCommand(
        string PlantaCode,
        string GuiaCode,
        string ArticuloCode,
        decimal Cantidad,
        bool IsVerified,
        byte[] RowVersion
    ) : IRequest<GuiaResponse>;
}
