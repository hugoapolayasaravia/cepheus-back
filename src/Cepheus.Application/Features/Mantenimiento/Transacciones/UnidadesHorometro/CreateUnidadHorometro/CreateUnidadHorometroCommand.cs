using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.Common;
using Cepheus.Domain.Mantenimiento.Enum;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.CreateUnidadHorometro
{
    public record CreateUnidadHorometroCommand(
        string SubCentroCostoCode,
        DateTime Fecha,
        decimal Horometro,
        EstadoEquipo Estado,
        string PlantaAfectadaCode,
        string? EquipoCode
    ) : IRequest<UnidadHorometroResponse>;
}
