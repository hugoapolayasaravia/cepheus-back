using Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.GetTipoCuentaByCode
{
    public record GetTipoCuentaByCodeQuery(string Code) : IRequest<TipoCuentaResponse>;
}