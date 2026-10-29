using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.GetTrabajadorSaludsByTrabajador
{
    public record GetTrabajadorSaludsByTrabajadorQuery(string TrabajadorCode) : IRequest<TrabajadorSaludResponse?>;
}
