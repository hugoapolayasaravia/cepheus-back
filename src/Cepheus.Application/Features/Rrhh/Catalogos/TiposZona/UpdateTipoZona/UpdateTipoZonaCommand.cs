using Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.UpdateTipoZona
{
    public record UpdateTipoZonaCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoZonaResponse>;
}