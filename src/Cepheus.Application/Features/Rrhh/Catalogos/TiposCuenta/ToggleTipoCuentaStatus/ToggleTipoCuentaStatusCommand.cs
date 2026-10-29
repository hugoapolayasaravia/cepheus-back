using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.ToggleTipoCuentaStatus
{
    public record ToggleTipoCuentaStatusCommand(string Code) : IRequest<bool>;
}