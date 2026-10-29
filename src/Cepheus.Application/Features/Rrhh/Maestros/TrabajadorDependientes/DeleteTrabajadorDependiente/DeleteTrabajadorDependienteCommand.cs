using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.DeleteTrabajadorDependiente
{
    public record DeleteTrabajadorDependienteCommand(long Id) : IRequest;
}
