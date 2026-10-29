using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.GetTrabajadorFormacionsByTrabajador
{
    public record GetTrabajadorFormacionsByTrabajadorQuery(string TrabajadorCode) : IRequest<List<TrabajadorFormacionResponse>>;
}
