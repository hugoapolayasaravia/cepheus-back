using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.GetAlturaLosaByCode
{
    public record GetAlturaLosaByCodeQuery(string Code) : IRequest<AlturaLosaResponse>;
}
