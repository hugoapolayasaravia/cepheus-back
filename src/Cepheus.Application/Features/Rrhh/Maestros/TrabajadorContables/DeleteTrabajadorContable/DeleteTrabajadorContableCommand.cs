using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.DeleteTrabajadorContable
{
    public record DeleteTrabajadorContableCommand(long Id) : IRequest;
}
