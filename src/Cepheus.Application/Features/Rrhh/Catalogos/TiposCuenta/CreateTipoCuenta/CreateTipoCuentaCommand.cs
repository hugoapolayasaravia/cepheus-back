using Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.CreateTipoCuenta
{
    public record CreateTipoCuentaCommand(
        string Name
    ) : IRequest<TipoCuentaResponse>;
}