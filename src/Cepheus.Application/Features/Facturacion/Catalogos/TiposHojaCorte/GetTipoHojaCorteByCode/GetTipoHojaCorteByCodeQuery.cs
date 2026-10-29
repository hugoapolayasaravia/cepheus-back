using Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.GetTipoHojaCorteByCode
{
    public record GetTipoHojaCorteByCodeQuery(string Code) : IRequest<TipoHojaCorteResponse>;
}
