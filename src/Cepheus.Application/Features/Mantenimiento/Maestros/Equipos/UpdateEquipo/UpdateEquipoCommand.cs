using Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.UpdateEquipo
{
    public record UpdateEquipoCommand(
        string Code,
        string Name,
        int Nivel,
        string? SubCentroCostoCode,
        byte[] RowVersion
    ) : IRequest<EquipoResponse>;
}
