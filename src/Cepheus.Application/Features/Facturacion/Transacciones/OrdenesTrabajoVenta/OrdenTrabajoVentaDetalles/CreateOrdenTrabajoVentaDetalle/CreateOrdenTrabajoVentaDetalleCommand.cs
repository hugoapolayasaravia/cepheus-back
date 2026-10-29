using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.OrdenTrabajoVentaDetalles.CreateOrdenTrabajoVentaDetalle;

public sealed record CreateOrdenTrabajoVentaDetalleCommand(string Code, OrdenTrabajoVentaDetalleRequest Data)
    : IRequest<OrdenTrabajoVentaResponse>;
