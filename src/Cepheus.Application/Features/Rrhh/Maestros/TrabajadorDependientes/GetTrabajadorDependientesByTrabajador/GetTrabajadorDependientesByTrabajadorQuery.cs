using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.GetTrabajadorDependientesByTrabajador
{
    public record GetTrabajadorDependientesByTrabajadorQuery(string TrabajadorCode) : IRequest<List<TrabajadorDependienteResponse>>;
}
