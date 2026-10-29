using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.ToggleTipoAnexoStatus
{
    public record ToggleTipoAnexoStatusCommand(string Code) : IRequest<bool>;
}
