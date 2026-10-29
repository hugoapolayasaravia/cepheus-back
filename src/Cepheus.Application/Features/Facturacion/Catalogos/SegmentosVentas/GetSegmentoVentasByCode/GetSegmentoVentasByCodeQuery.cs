using Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.GetSegmentoVentasByCode
{
    public record GetSegmentoVentasByCodeQuery(string Code) : IRequest<SegmentoVentasResponse>;
}
