using Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.GetTipoZonaByCode
{
    public record GetTipoZonaByCodeQuery(string Code) : IRequest<TipoZonaResponse>;
}