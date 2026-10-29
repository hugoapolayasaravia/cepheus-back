using Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.CreateTipoAfiliacion
{
    public record CreateTipoAfiliacionCommand(
        string Name
    ) : IRequest<TipoAfiliacionResponse>;
}