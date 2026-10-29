using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.GetTrabajadorSegurosByTrabajador
{
    public record GetTrabajadorSegurosByTrabajadorQuery(string TrabajadorCode) : IRequest<TrabajadorSeguroResponse?>;
}
