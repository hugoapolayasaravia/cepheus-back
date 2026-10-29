using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.DeleteTrabajadorContrato
{
    public record DeleteTrabajadorRemuneracionCommand(long Id) : IRequest;
}
