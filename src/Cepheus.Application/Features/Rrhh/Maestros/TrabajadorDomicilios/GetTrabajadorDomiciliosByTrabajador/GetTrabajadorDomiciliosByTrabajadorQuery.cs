using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.GetTrabajadorDomiciliosByTrabajador
{
    public record GetTrabajadorDomiciliosByTrabajadorQuery(string TrabajadorCode) : IRequest<List<TrabajadorDomicilioResponse>>;
}