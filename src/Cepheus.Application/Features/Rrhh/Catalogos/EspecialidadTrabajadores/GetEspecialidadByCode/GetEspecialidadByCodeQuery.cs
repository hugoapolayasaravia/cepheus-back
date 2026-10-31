using Cepheus.Application.Features.Rrhh.Catalogos.EspecialidadTrabajadores.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EspecialidadTrabajadores.GetEspecialidadByCode
{
    public record GetEspecialidadByCodeQuery(string Code) : IRequest<EspecialidadTrabajadorResponse>;
}