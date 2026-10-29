using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.GetTrabajadorVacacionsByTrabajador
{
    public record GetTrabajadorVacacionsByTrabajadorQuery(string TrabajadorCode) : IRequest<List<TrabajadorVacacionResponse>>;
}
