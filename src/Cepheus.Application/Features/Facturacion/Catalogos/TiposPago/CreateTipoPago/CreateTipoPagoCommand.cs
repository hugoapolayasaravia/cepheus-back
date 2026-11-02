using Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.CreateTipoPago
{
    /// <summary>
    /// El código lo define el usuario (legacy: 01 Efectivo, 02 Cheque, 03 Letras, ..., 11 Letras Varias): el módulo de
    /// Cancelaciones usa esos códigos con reglas propias.
    /// </summary>
    public record CreateTipoPagoCommand(
        string Code, string Name, int SortOrder, bool ShowInApproval, List<TipoPagoCuentaInput>? Accounts)
        : IRequest<TipoPagoResponse>;
}
