using Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.GetObjetoActividadByCode
{
    public record GetObjetoActividadByCodeQuery(string Code) : IRequest<ObjetoActividadResponse>;
}
