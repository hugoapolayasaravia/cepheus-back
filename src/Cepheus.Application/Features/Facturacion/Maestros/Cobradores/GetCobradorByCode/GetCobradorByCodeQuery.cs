using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Cobradores.GetCobradorByCode
{
    public record GetCobradorByCodeQuery(string Code) : IRequest<CobradorResponse>;
}
