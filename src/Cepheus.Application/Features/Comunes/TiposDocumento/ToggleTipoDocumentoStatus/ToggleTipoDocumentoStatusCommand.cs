using MediatR;

namespace Cepheus.Application.Features.Comunes.TiposDocumento.ToggleTipoDocumentoStatus
{
    public record ToggleTipoDocumentoStatusCommand(string Code) : IRequest<bool>;
}
