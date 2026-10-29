using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.ToggleTipoPensionStatus
{
    public record ToggleTipoPensionStatusCommand(string Code) : IRequest<bool>;
}