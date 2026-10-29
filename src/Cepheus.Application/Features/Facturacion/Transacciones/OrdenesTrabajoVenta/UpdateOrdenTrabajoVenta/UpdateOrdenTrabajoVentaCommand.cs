using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.UpdateOrdenTrabajoVenta;

/// <summary>Modifica la cabecera. El código va en la ruta; cliente, obra, tipo, origen y cotización no cambian.</summary>
public sealed record UpdateOrdenTrabajoVentaCommand(string Code, OrdenTrabajoVentaUpdateFields Data)
    : IRequest<OrdenTrabajoVentaResponse>;
