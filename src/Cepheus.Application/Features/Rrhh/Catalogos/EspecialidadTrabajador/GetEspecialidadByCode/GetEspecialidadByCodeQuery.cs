using Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.GetEspecialidadByCode
{
    public record GetEspecialidadByCodeQuery(string Code) : IRequest<EspecialidadTrabajadorResponse>;
}