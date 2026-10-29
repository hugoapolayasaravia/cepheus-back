using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.DesbloquearOrdenTrabajoVenta;

/// <summary>Devuelve la OT bloqueada a modificable (solo si sigue pendiente de aprobación).</summary>
public sealed record DesbloquearOrdenTrabajoVentaCommand(string Code) : IRequest<OrdenTrabajoVentaResponse>;
