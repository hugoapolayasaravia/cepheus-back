using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.DeleteTrabajadorLaboral
{
    public record DeleteTrabajadorLaboralCommand(long Id) : IRequest;
}
