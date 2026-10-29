using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposVale.ToggleTipoValeStatus
{
    public record ToggleTipoValeStatusCommand(string Code) : IRequest<bool>;
}