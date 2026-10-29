using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.ToggleUnidadMedidaStatus
{
    public record ToggleUnidadMedidaStatusCommand(string Code) : IRequest<bool>;
}