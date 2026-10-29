using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.GetCreditoOrdenTrabajoVenta;

public sealed class CreditoOrdenTrabajoVentaResponse
{
    public string OrdenTrabajoCode { get; set; } = default!;
    public string ClienteCode { get; set; } = default!;
    public string ObraCode { get; set; } = default!;
    public decimal OrderTotal { get; set; }
    public string Mode { get; set; } = default!;
    public decimal Limit { get; set; }
    public decimal Used { get; set; }
    public decimal Available { get; set; }
    /// <summary>Si la regla del PB aplica a esta OT (no Descuento, cliente no VIP, obra a crédito y no proyecto).</summary>
    public bool Required { get; set; }
    public bool Covers { get; set; }
    public string? Note { get; set; }
}

public sealed record GetCreditoOrdenTrabajoVentaQuery(string Code) : IRequest<CreditoOrdenTrabajoVentaResponse>;
