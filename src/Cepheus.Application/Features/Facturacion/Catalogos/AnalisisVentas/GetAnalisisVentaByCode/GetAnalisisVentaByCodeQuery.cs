using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.GetAnalisisVentaByCode
{
    public record GetAnalisisVentaByCodeQuery(string Code) : IRequest<AnalisisVentaResponse>;
}
