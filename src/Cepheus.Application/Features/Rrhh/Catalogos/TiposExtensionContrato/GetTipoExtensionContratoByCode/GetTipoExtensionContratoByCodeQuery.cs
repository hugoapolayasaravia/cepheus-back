using Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.GetTipoExtensionContratoByCode
{
    public record GetTipoExtensionContratoByCodeQuery(string Code) : IRequest<TipoExtensionContratoResponse>;
}