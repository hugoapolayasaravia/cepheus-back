using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.GetTrabajadorPensionsByTrabajador
{
    public record GetTrabajadorPensionsByTrabajadorQuery(string TrabajadorCode) : IRequest<TrabajadorPensionResponse?>;
}
