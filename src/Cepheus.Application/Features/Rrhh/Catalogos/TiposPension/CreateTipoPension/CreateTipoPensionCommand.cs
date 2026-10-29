using Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.CreateTipoPension
{
    public record CreateTipoPensionCommand(
        string Name
    ) : IRequest<TipoPensionResponse>;
}