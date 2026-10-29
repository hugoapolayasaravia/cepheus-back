using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.GetOrdenTrabajoVentaByCode;

public sealed record GetOrdenTrabajoVentaByCodeQuery(string Code) : IRequest<OrdenTrabajoVentaResponse>;
