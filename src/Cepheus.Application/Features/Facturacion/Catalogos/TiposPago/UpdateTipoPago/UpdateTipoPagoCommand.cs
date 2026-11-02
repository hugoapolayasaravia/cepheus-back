using Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.UpdateTipoPago
{
    /// <summary>Reemplaza el nombre, el orden, "visible en aprobación" y TODAS las cuentas por moneda del tipo de pago.</summary>
    public record UpdateTipoPagoCommand(
        string Code, string Name, int SortOrder, bool ShowInApproval, List<TipoPagoCuentaInput>? Accounts, byte[] RowVersion)
        : IRequest<TipoPagoResponse>;
}
