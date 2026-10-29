// Cepheus.Application/Features/Logistica/Transacciones/Guias/GuiaDetalles/DeleteGuiaDetalle/DeleteGuiaDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.GuiaDetalles.DeleteGuiaDetalle
{
    public record DeleteGuiaDetalleCommand(
        string PlantaCode,
        string GuiaCode,
        string ArticuloCode
    ) : IRequest<GuiaResponse>;
}
