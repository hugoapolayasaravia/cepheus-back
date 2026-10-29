using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.GetTrabajadorRemuneracionsByTrabajador
{
    public record GetTrabajadorRemuneracionsByTrabajadorQuery(string TrabajadorCode) : IRequest<TrabajadorRemuneracionResponse?>;
}
