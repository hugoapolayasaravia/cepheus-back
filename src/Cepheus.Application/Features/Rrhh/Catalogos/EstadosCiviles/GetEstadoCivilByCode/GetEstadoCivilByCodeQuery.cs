using Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.GetEstadoCivilByCode
{
    public record GetEstadoCivilByCodeQuery(string Code) : IRequest<EstadoCivilResponse>;
}