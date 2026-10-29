using Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.GetActividadByCode
{
    public record GetActividadByCodeQuery(string Code) : IRequest<ActividadResponse>;
}
