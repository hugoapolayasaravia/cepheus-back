using Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.CreateTipoContrato
{
    public record CreateTipoContratoCommand(
        string Name
    ) : IRequest<TipoContratoResponse>;
}