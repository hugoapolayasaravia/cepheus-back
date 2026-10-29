using Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.GetTipoPensionByCode
{
    public record GetTipoPensionByCodeQuery(string Code) : IRequest<TipoPensionResponse>;
}