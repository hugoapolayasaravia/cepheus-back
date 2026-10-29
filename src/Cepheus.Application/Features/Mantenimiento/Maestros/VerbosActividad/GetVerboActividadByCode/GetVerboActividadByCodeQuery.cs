using Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.GetVerboActividadByCode
{
    public record GetVerboActividadByCodeQuery(string Code) : IRequest<VerboActividadResponse>;
}
