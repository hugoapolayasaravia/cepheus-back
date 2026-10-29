using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.ToggleTipoCuentaContableStatus
{
    public record ToggleTipoCuentaContableStatusCommand(string Code) : IRequest<bool>;
}
