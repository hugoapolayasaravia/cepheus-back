using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.ToggleTipoHojaCorteStatus
{
    public record ToggleTipoHojaCorteStatusCommand(string Code) : IRequest<bool>;
}
