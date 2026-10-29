// Cepheus.Application/Features/Logistica/Transacciones/CotizacionDetalles/CreateCotizacionDetalle/CreateCotizacionDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionDetalles.CreateCotizacionDetalle
{
    public record CreateCotizacionDetalleCommand(
        string PlantaCode,
        string CotizacionCode,
        string ArticuloCode,
        decimal? CantidadArticulo,
        List<CreateCotizacionDetalleOrigenInput>? Origenes
    ) : IRequest<CotizacionResponse>;

    public record CreateCotizacionDetalleOrigenInput(string PedidoCode, int PedidoItemNumber, decimal CantidadTomada);
}