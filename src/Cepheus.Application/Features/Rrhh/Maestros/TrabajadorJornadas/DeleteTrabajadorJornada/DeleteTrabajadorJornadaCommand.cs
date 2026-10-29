using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorJornadas.DeleteTrabajadorJornada
{
    public record DeleteTrabajadorJornadaCommand(long Id) : IRequest;
}
