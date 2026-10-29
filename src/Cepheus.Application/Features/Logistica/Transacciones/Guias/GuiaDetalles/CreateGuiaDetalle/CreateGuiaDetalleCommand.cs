// Cepheus.Application/Features/Logistica/Transacciones/Guias/GuiaDetalles/CreateGuiaDetalle/CreateGuiaDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.GuiaDetalles.CreateGuiaDetalle
{
    /// <summary>Agrega una línea a una Guía existente en estado Pendiente.</summary>
    public record CreateGuiaDetalleCommand(
        string PlantaCode,
        string GuiaCode,
        string ArticuloCode,
        decimal Cantidad,
        bool IsVerified
    ) : IRequest<GuiaResponse>;
}
