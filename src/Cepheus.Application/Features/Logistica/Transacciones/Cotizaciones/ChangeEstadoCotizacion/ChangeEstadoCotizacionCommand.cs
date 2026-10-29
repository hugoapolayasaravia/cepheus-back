// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/ChangeEstadoCotizacion/ChangeEstadoCotizacionCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.ChangeEstadoCotizacion
{
    public record ChangeEstadoCotizacionCommand(string PlantaCode, string Code, string NuevoEstado) : IRequest<CotizacionResponse>;
}