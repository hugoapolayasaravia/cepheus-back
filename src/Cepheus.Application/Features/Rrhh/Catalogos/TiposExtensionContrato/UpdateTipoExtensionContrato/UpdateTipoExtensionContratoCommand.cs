using Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.UpdateTipoExtensionContrato
{
    public record UpdateTipoExtensionContratoCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoExtensionContratoResponse>;
}