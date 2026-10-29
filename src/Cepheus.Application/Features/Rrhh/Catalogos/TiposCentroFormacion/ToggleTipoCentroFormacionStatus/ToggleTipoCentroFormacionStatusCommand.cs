using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.ToggleTipoCentroFormacionStatus
{
    public record ToggleTipoCentroFormacionStatusCommand(string Code) : IRequest<bool>;
}