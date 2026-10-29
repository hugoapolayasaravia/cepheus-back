using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.ToggleTipoAsientoStatus
{
    public record ToggleTipoAsientoStatusCommand(string Code) : IRequest<bool>;
}
