using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.DeleteTrabajadorDomicilio
{
    public record DeleteTrabajadorDomicilioCommand(int Id) : IRequest;
}