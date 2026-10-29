// Cepheus.Application/Features/Logistica/Transacciones/Guias/CreateGuia/CreateGuiaCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.CreateGuia
{
    /// <summary>
    /// No recibe Code (el número 'SSS-NNNNNN' lo genera el backend desde
    /// Planta.GuiaNum), Estado (nace en Pendiente) ni usuario/fecha de proceso
    /// (los cubre la auditoría). PuntoPartida es opcional: si no se envía se
    /// usa la dirección de la planta (comportamiento del legacy).
    /// Requiere al menos una línea de detalle y no admite artículos repetidos.
    /// </summary>
    public record CreateGuiaCommand(
        string PlantaCode,
        DateTime FechaEmision,
        string Hora,
        string ProveedorCode,
        string MotivoCode,
        string Direccion,
        string? PuntoPartida,
        string TransportistaCode,
        string ConductorCode,
        string VehiculoCode,
        string? Observaciones,
        List<CreateGuiaDetalleLineaInput> Detalles
    ) : IRequest<GuiaResponse>;

    public record CreateGuiaDetalleLineaInput(
        string ArticuloCode,
        decimal Cantidad,
        bool IsVerified
    );
}
