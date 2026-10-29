using Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.UpdateTipoPension
{
    public record UpdateTipoPensionCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoPensionResponse>;
}