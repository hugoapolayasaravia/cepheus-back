using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.OrdenTrabajoVentaDetalles.DeleteOrdenTrabajoVentaDetalle;

/// <summary>Elimina una línea y devuelve su cantidad a la cotización (si la hay).</summary>
public sealed record DeleteOrdenTrabajoVentaDetalleCommand(string Code, int Item) : IRequest<OrdenTrabajoVentaResponse>;
