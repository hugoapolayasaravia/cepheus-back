using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.PasarAProduccionOrdenTrabajoVenta;

/// <summary>Pase a producción de una OT aprobada (PB: Ind_PaseProduccion = 'S' y FechaPaseProduccion).</summary>
public sealed record PasarAProduccionOrdenTrabajoVentaCommand(string Code) : IRequest<OrdenTrabajoVentaResponse>;
