using Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.CreateEquipo
{
    public record CreateEquipoCommand(
        string Code,
        string Name,
        int Nivel,
        string? SubCentroCostoCode
    ) : IRequest<EquipoResponse>;
}
