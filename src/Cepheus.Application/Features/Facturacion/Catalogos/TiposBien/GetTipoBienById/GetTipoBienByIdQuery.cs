using Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.GetTipoBienById
{
    public record GetTipoBienByIdQuery(string Code) : IRequest<TipoBienResponse>;
}
