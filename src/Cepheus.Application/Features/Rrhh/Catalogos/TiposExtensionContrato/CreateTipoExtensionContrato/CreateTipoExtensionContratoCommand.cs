using Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.CreateTipoExtensionContrato
{
    public record CreateTipoExtensionContratoCommand(
        string Name
    ) : IRequest<TipoExtensionContratoResponse>;
}