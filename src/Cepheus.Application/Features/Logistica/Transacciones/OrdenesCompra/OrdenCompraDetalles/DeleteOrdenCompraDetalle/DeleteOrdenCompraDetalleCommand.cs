// Cepheus.Application/Features/Logistica/Transacciones/OrdenCompraDetalles/DeleteOrdenCompraDetalle/DeleteOrdenCompraDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenCompraDetalles.DeleteOrdenCompraDetalle
{
    public record DeleteOrdenCompraDetalleCommand(string PlantaCode, string OrdenCompraCode, string ArticuloCode) : IRequest<OrdenCompraResponse>;
}