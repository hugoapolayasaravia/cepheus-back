using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.UpdateTrabajadorSalud
{
    public record UpdateTrabajadorSaludCommand(
        long Id,
        string? TrabajadorCode,
        string? TipoSangreCode,
        string? AlergiaCode,
        string? Otros,
        DateTime? FechaEvaluacionMedica,
        string? Observaciones,
        byte[] RowVersion
    ) : IRequest<TrabajadorSaludResponse>;
}
