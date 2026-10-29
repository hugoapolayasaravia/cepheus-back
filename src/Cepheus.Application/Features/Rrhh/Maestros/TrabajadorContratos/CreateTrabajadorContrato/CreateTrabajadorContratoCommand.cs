using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.CreateTrabajadorContrato
{
    public record CreateTrabajadorContratoCommand(
        string TrabajadorCode,
        string? TipoContratoCode,
        string? TipoExtensionCode,
        DateTime? FechaInicio,
        DateTime? FechaFin,
        DateTime? FechaTermino,
        bool? Renovado,
        string? TipoDuracion,
        int? CantidadDuracion,
        bool Activo
    ) : IRequest<TrabajadorContratoResponse>;
}
