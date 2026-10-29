// Cepheus.Application/Features/Logistica/Transacciones/CotizacionDetalles/DeleteCotizacionDetalle/DeleteCotizacionDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionDetalles.DeleteCotizacionDetalle
{
    public record DeleteCotizacionDetalleCommand(string PlantaCode, string CotizacionCode, string ArticuloCode) : IRequest<CotizacionResponse>;
}