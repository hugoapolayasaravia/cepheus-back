using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.UpdateTrabajadorLaboral
{
    public record UpdateTrabajadorLaboralCommand(
        long Id,
        string TrabajadorCode,
        DateTime? FechaIngreso,
        DateTime? FechaCese,
        string? TipoTrabajadorCode,
        string? CategoriaTrabajadorCode,
        string? EstadoTrabajadorCode,
        string? AreaCode,
        string? OcupacionCode,
        string? SubOcupacionCode,
        string? OficinaCode,
        string? PlantaCode,
        string? CargoCode,
        string? NivelCode,
        string? RegimenLaboralCode,
        string? ProveedorCode,
        bool? Permanente,
        bool? Pensionista,
        byte[] RowVersion
    ) : IRequest<TrabajadorLaboralResponse>;
}
