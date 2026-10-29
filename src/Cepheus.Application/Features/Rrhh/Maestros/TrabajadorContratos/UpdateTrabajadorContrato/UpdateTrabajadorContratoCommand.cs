using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.UpdateTrabajadorContrato
{
    public record UpdateTrabajadorContratoCommand(
        long Id,
        string? TrabajadorCode,
        string? TipoContratoCode,
        string? TipoExtensionCode,
        DateTime? FechaInicio,
        DateTime? FechaFin,
        DateTime? FechaTermino,
        bool? Renovado,
        string? TipoDuracion,
        int? CantidadDuracion,
        bool Activo,
        byte[] RowVersion
    ) : IRequest<TrabajadorContratoResponse>;
}
