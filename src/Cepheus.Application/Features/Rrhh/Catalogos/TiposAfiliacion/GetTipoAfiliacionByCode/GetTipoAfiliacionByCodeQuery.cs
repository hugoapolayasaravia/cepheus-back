using Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.GetTipoAfiliacionByCode
{
    public record GetTipoAfiliacionByCodeQuery(string Code) : IRequest<TipoAfiliacionResponse>;
}