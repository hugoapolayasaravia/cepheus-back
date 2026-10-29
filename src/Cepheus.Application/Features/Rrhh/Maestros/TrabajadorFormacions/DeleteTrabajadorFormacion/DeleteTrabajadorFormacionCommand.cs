using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.DeleteTrabajadorFormacion
{
    public record DeleteTrabajadorFormacionCommand(long Id) : IRequest;
}
