using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.CreateTrabajadorPension
{
    public record CreateTrabajadorPensionCommand(
        string TrabajadorCode,
        string? TipoAfiliacionCode,
        string? AfpCode,
        DateTime? FechaAfiliacion,
        string? NumeroAfp,
        string? RegimenPensionarioCode,
        string? TipoPensionCode,
        string? NumeroCarnetSsp
    ) : IRequest<TrabajadorPensionResponse>;
}
