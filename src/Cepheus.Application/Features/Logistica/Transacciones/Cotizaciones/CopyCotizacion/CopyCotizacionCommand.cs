// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/CopyCotizacion/CopyCotizacionCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.CopyCotizacion
{
    public record CopyCotizacionCommand(string PlantaCode, string Code, DateTime NuevaFechaLimite) : IRequest<CotizacionResponse>;
}