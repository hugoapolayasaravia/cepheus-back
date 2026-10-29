using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.ToggleTipoExtensionContratoStatus
{
    public record ToggleTipoExtensionContratoStatusCommand(string Code) : IRequest<bool>;
}