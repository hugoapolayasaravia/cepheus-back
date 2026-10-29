using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.Common;
using Cepheus.Domain.Mantenimiento.Enum;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.UpdateUnidadParada
{
    /// <summary>
    /// Permite corregir Horas, estado, planta afectada y falla; la clave
    /// (subcentro, fecha) no se edita.
    /// </summary>
    public record UpdateUnidadParadaCommand(
        string SubCentroCostoCode,
        DateTime Fecha,
        decimal Horas,
        EstadoEquipo Estado,
        string PlantaAfectadaCode,
        string? FallaCode,
        byte[] RowVersion
    ) : IRequest<UnidadParadaResponse>;
}
