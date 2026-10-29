using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorJornadas.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorJornadas.GetTrabajadorJornadasByTrabajador
{
    public record GetTrabajadorJornadasByTrabajadorQuery(string TrabajadorCode) : IRequest<TrabajadorJornadaResponse?>;
}
