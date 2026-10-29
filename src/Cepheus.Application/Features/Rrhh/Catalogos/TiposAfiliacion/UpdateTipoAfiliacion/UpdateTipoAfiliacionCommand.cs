using Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.UpdateTipoAfiliacion
{
    public record UpdateTipoAfiliacionCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoAfiliacionResponse>;
}