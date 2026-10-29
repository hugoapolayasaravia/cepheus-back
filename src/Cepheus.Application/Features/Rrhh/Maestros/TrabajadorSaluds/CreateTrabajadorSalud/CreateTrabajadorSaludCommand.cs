using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.CreateTrabajadorSalud
{
    public record CreateTrabajadorSaludCommand(
        string TrabajadorCode,
        string? TipoSangreCode,
        string? AlergiaCode,
        string? Otros,
        DateTime? FechaEvaluacionMedica,
        string? Observaciones
    ) : IRequest<TrabajadorSaludResponse>;
}
