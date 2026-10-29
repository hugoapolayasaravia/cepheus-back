using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.TogglePlantillaAsientoStatus
{
    public record TogglePlantillaAsientoStatusCommand(string Code) : IRequest<bool>;
}
