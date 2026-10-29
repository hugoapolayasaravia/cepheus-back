using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.ToggleSubCentroCostoStatus
{
    public record ToggleSubCentroCostoStatusCommand(string Code) : IRequest<bool>;
}
