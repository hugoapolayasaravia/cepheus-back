using Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.UpdateTipoHojaCorte
{
    public record UpdateTipoHojaCorteCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoHojaCorteResponse>;
}
