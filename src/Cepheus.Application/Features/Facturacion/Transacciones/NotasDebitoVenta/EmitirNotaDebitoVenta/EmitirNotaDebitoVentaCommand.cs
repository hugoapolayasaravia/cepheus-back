using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.EmitirNotaDebitoVenta
{
    /// <summary>
    /// Emite la nota de débito (equivale a la impresión definitiva del legacy, ue_set_btn_imp):
    /// genera el asiento de provisión y la pasa a Facturado marcando impresión y fecha.
    /// </summary>
    public record EmitirNotaDebitoVentaCommand(string PlantaCode, string Number) : IRequest<NotaDebitoVentaResponse>;
}
