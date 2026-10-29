using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.GetTrabajadorLaboralsByTrabajador
{
    public record GetTrabajadorLaboralsByTrabajadorQuery(string TrabajadorCode) : IRequest<TrabajadorLaboralResponse?>;
}
