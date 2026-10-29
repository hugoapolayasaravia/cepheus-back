using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.UpdateTipoCuentaContable
{
    public record UpdateTipoCuentaContableCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoCuentaContableResponse>;
}
