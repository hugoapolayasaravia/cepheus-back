using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.GetEspecialidadByCode
{
    public record GetEspecialidadByCodeQuery(string Code) : IRequest<EspecialidadResponse>;
}
