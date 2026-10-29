// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/CreatePedido/CreatePedidoCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.CreatePedido
{
    /// <summary>
    /// No recibe Code (correlativo '7' + 5 dígitos por planta, lo genera el
    /// backend), Estado (nace en Pendiente) ni Neto/Igv/Total (se calculan
    /// desde las líneas). Usuario se cubre con CreatedBy (auditoría).
    /// Requiere al menos una línea de detalle.
    /// </summary>
    public record CreatePedidoCommand(
        string PlantaCode,
        string TipoPedidoCode,
        string TipoValeCode,
        string TramiteCode,
        string SubCentroCostoCode,
        string TrabajadorCode,
        string? OrdenTrabajoCode,
        string UnidadNegocioCode,
        DateTime FechaEntrega,
        string? Observaciones,
        List<CreatePedidoDetalleLineaInput> Detalles
    ) : IRequest<PedidoResponse>;

    public record CreatePedidoDetalleLineaInput(
        string? ArticuloCode,
        string DescripcionArticulo,
        string UnidadMedidaCode,
        decimal PrecioArticulo,
        decimal CantidadArticulo,
        string? ProveedorCode
    );
}