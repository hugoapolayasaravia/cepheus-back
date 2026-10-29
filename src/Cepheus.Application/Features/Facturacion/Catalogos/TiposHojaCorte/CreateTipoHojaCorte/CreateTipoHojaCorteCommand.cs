using Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.CreateTipoHojaCorte
{
    public record CreateTipoHojaCorteCommand(
        string Name
    ) : IRequest<TipoHojaCorteResponse>;
}
