using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.Common;
using Cepheus.Domain.Mantenimiento.Enum;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.CreateUnidadParada
{
    /// <summary>
    /// Registra la parada de una unidad en un día (legacy Logi_sp_AgregaMUnidad_Parada), con semántica de
    /// "agregar o actualizar":
    ///   - Horas = 0: no hace nada y devuelve null (el SP legado hacía RETURN sin grabar).
    ///   - No existe registro para (subcentro, fecha): lo inserta.
    ///   - Ya existe: actualiza Horas, Estado, PlantaAfectada y Falla (el último dato gana, sin RowVersion,
    ///     igual que el SP legado).
    /// </summary>
    public record CreateUnidadParadaCommand(
        string SubCentroCostoCode,
        DateTime Fecha,
        decimal Horas,
        EstadoEquipo Estado,
        string PlantaAfectadaCode,
        string? FallaCode
    ) : IRequest<UnidadParadaResponse?>;
}
