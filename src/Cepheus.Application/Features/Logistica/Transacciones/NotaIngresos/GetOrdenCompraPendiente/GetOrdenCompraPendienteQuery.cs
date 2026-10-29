using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetOrdenCompraPendiente;

/// <summary>Datos de una Orden de Compra para recibirla: cabecera y lo pendiente por artículo / pedido.</summary>
public sealed record GetOrdenCompraPendienteQuery(string PlantaCode, string OrdenCompraCode)
    : IRequest<OrdenCompraPendienteResponse>;

public sealed class OrdenCompraPendienteLineaResponse
{
    public string ArticuloCode { get; set; } = default!;
    public string ArticuloName { get; set; } = default!;
    public string UnidadMedidaCode { get; set; } = default!;
    public string? PedidoCode { get; set; }
    public decimal CantidadPendiente { get; set; }

    /// <summary>Precio de la OC; la Nota de Ingreso no puede superarlo.</summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// Porcentaje (0-100) equivalente al descuento de la OC. En la OC el descuento es un monto; la
    /// Nota de Ingreso (como el legacy) lo maneja en porcentaje: monto / (cantidad × precio) × 100.
    /// </summary>
    public decimal DescuentoPorcentaje { get; set; }
}

public sealed class OrdenCompraPendienteResponse
{
    public string PlantaCode { get; set; } = default!;
    public string OrdenCompraCode { get; set; } = default!;
    public EstadoOrdenCompra Estado { get; set; }
    public string ProveedorCode { get; set; } = default!;
    public string ProveedorName { get; set; } = default!;
    public string MonedaCode { get; set; } = default!;
    public string FormaPagoCode { get; set; } = default!;
    public string FormaPagoName { get; set; } = default!;
    public List<OrdenCompraPendienteLineaResponse> Lineas { get; set; } = new();
}
