using Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.GetOficinaByCode
{
    public record GetOficinaByCodeQuery(string Code) : IRequest<OficinaResponse>;
}