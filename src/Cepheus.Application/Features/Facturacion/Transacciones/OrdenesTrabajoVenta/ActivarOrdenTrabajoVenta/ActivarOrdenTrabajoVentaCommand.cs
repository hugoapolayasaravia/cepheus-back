using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.ActivarOrdenTrabajoVenta;

/// <summary>Reactiva una OT Inactiva (PB: Codigo_est 06 -> 05).</summary>
public sealed record ActivarOrdenTrabajoVentaCommand(string Code) : IRequest<OrdenTrabajoVentaResponse>;
