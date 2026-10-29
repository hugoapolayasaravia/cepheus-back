using Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.UpdateTipoCuenta
{
    public record UpdateTipoCuentaCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoCuentaResponse>;
}