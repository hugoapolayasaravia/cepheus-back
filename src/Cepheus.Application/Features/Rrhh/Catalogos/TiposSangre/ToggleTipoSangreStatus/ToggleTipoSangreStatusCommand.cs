using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.ToggleTipoSangreStatus
{
    public record ToggleTipoSangreStatusCommand(string Code) : IRequest<bool>;
}