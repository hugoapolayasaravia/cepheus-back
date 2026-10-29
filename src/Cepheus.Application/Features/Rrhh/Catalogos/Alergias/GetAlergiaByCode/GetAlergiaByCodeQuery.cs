using Cepheus.Application.Features.Rrhh.Catalogos.Alergias.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Alergias.GetAlergiaByCode
{
    public record GetAlergiaByCodeQuery(string Code) : IRequest<AlergiaResponse>;
}