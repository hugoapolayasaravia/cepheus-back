using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.ToggleTipoSctrStatus
{
    public record ToggleTipoSctrStatusCommand(string Code) : IRequest<bool>;
}