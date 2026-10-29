using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.CreateTrabajadorVacacion
{
    public record CreateTrabajadorVacacionCommand(
        string TrabajadorCode,
        DateTime? FechaVacaciones
    ) : IRequest<TrabajadorVacacionResponse>;
}
