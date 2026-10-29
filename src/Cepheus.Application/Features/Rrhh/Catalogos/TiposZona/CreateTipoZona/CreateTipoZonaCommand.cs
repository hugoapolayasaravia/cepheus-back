using Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.CreateTipoZona
{
    public record CreateTipoZonaCommand(
        string Name
    ) : IRequest<TipoZonaResponse>;
}