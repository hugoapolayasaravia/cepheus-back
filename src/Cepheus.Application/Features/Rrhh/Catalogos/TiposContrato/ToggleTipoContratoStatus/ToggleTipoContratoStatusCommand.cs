using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.ToggleTipoContratoStatus
{
    public record ToggleTipoContratoStatusCommand(string Code) : IRequest<bool>;
}