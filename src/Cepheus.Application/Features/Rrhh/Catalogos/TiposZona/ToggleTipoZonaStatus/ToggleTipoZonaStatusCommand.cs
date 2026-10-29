using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.ToggleTipoZonaStatus
{
    public record ToggleTipoZonaStatusCommand(string Code) : IRequest<bool>;
}