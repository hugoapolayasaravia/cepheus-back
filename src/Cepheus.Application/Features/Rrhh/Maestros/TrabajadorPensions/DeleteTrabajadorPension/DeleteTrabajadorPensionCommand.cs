using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.DeleteTrabajadorPension
{
    public record DeleteTrabajadorPensionCommand(long Id) : IRequest;
}
