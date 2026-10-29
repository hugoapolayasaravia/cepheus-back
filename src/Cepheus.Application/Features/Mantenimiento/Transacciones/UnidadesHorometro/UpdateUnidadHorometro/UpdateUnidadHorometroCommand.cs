using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.Common;
using Cepheus.Domain.Mantenimiento.Enum;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.UpdateUnidadHorometro
{
    /// <summary>
    /// Permite corregir Horometro, estado, planta afectada y equipo; la clave
    /// (subcentro, fecha) no se edita.
    /// </summary>
    public record UpdateUnidadHorometroCommand(
        string SubCentroCostoCode,
        DateTime Fecha,
        decimal Horometro,
        EstadoEquipo Estado,
        string PlantaAfectadaCode,
        string? EquipoCode,
        byte[] RowVersion
    ) : IRequest<UnidadHorometroResponse>;
}
