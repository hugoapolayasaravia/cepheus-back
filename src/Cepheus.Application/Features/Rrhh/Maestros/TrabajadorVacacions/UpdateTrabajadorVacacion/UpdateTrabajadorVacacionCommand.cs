using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.UpdateTrabajadorVacacion
{
    public record UpdateTrabajadorVacacionCommand(
        long Id,
        string? TrabajadorCode,
        DateTime? FechaVacaciones,
        byte[] RowVersion
    ) : IRequest<TrabajadorVacacionResponse>;
}
