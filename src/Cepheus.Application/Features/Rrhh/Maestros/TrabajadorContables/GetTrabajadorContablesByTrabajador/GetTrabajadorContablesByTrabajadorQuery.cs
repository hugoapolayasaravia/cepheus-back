using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.GetTrabajadorContablesByTrabajador
{
    public record GetTrabajadorContablesByTrabajadorQuery(string TrabajadorCode) : IRequest<List<TrabajadorContableResponse>>;
}
