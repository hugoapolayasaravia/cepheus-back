using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.ToggleTipoViaStatus
{
    public record ToggleTipoViaStatusCommand(string Code) : IRequest<bool>;
}