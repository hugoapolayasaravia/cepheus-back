using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.UpdateTrabajadorPension
{
    public record UpdateTrabajadorPensionCommand(
        long Id,
        string? TrabajadorCode,
        string? TipoAfiliacionCode,
        string? AfpCode,
        DateTime? FechaAfiliacion,
        string? NumeroAfp,
        string? RegimenPensionarioCode,
        string? TipoPensionCode,
        string? NumeroCarnetSsp,
        byte[] RowVersion
    ) : IRequest<TrabajadorPensionResponse>;
}
