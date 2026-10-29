using Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.UpdateTipoSctr
{
    public record UpdateTipoSctrCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoSctrResponse>;
}