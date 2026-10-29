using Cepheus.Application.Features.Logistica.Catalogos.TiposVale.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposVale.GetTipoValeByCode
{
    public record GetTipoValeByCodeQuery(string Code) : IRequest<TipoValeResponse>;
}