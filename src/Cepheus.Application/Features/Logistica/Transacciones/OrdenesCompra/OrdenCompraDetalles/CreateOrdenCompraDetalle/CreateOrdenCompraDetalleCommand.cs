// Cepheus.Application/Features/Logistica/Transacciones/OrdenCompraDetalles/CreateOrdenCompraDetalle/CreateOrdenCompraDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenCompraDetalles.CreateOrdenCompraDetalle
{
    public record CreateOrdenCompraDetalleCommand(
        string PlantaCode,
        string OrdenCompraCode,
        string ArticuloCode,
        decimal CantidadArticulo,
        decimal PrecioArticulo,
        decimal DescuentoArticulo,
        string SubCentroCostoCode,
        List<CreateOrdenCompraDetalleOrigenInput>? Origenes
    ) : IRequest<OrdenCompraResponse>;

    public record CreateOrdenCompraDetalleOrigenInput(string PedidoCode, int PedidoItemNumber, decimal CantidadTomada);
}