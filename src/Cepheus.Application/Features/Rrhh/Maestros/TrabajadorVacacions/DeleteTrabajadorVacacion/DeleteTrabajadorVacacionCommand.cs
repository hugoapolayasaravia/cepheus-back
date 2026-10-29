using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.DeleteTrabajadorVacacion
{
    public record DeleteTrabajadorVacacionCommand(long Id) : IRequest;
}
