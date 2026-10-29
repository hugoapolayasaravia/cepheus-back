using Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.CreateTipoBien
{
    public record CreateTipoBienCommand(
        string Name,
        decimal DetractionRate
    ) : IRequest<TipoBienResponse>;
}
