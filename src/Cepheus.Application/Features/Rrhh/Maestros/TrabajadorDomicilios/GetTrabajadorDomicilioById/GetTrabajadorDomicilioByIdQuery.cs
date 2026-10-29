using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.GetTrabajadorDomicilioById
{
    public record GetTrabajadorDomicilioByIdQuery(int Id) : IRequest<TrabajadorDomicilioResponse>;
}