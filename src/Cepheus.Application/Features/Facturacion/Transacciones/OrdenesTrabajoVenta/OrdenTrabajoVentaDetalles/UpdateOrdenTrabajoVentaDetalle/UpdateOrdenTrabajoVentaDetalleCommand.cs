using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.OrdenTrabajoVentaDetalles.UpdateOrdenTrabajoVentaDetalle;

public sealed record UpdateOrdenTrabajoVentaDetalleCommand(string Code, int Item, UpdateOrdenTrabajoVentaDetalleRequest Data)
    : IRequest<OrdenTrabajoVentaResponse>;
