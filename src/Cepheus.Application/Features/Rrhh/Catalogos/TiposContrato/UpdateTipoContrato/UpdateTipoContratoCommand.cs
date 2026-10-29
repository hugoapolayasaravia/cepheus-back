using Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.UpdateTipoContrato
{
    public record UpdateTipoContratoCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoContratoResponse>;
}