using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.GetTrabajadorSindicatosByTrabajador
{
    public record GetTrabajadorSindicatosByTrabajadorQuery(string TrabajadorCode) : IRequest<TrabajadorSindicatoResponse?>;
}
