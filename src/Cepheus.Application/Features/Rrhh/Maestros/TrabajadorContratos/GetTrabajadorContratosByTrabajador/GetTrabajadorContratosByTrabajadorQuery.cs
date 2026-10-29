using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.GetTrabajadorContratosByTrabajador
{
    public record GetTrabajadorContratosByTrabajadorQuery(string TrabajadorCode) : IRequest<List<TrabajadorContratoResponse>>;
}
