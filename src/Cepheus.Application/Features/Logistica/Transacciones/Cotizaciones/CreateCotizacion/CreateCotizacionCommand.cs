// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/CreateCotizacion/CreateCotizacionCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.CreateCotizacion
{
    /// <summary>
    /// Cada línea (CreateCotizacionDetalleLineaInput) es de una de dos
    /// formas, mutuamente excluyentes:
    ///   - Origenes con datos -> CantidadArticulo se calcula como la suma
    ///     de CantidadTomada (confirmado: por ahora siempre así cuando hay
    ///     pedidos de origen).
    ///   - Origenes vacío/null -> CantidadArticulo se toma del valor
    ///     explícito enviado (cotización standalone, sin pedido).
    /// </summary>
    public record CreateCotizacionCommand(
        string PlantaCode,
        DateTime FechaLimite,
        string? Observaciones,
        List<CreateCotizacionDetalleLineaInput> Detalles
    ) : IRequest<CotizacionResponse>;

    public record CreateCotizacionDetalleLineaInput(
        string ArticuloCode,
        decimal? CantidadArticulo,
        List<CreateCotizacionPedidoOrigenInput>? Origenes
    );

    public record CreateCotizacionPedidoOrigenInput(
        string PedidoCode,
        int PedidoItemNumber,
        decimal CantidadTomada
    );
}