using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.GetTipoCuentaContableById
{
    public record GetTipoCuentaContableByIdQuery(string Code) : IRequest<TipoCuentaContableResponse>;
}
