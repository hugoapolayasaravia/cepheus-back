using Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.UpdateTipoBien
{
    public record UpdateTipoBienCommand(
        string Code,
        string Name,
        decimal DetractionRate,
        byte[] RowVersion
    ) : IRequest<TipoBienResponse>;
}
