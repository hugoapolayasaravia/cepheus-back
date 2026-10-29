using Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.CreateTipoSctr
{
    public record CreateTipoSctrCommand(
        string Name
    ) : IRequest<TipoSctrResponse>;
}