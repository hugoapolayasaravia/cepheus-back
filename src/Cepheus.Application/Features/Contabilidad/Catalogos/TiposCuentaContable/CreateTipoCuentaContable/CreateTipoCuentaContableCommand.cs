using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.CreateTipoCuentaContable
{
    public record CreateTipoCuentaContableCommand(
        string Name
    ) : IRequest<TipoCuentaContableResponse>;
}
