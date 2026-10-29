// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/UpdateCotizacion/UpdateCotizacionCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.UpdateCotizacion
{
    public record UpdateCotizacionCommand(
        string PlantaCode,
        string Code,
        DateTime FechaLimite,
        string? Observaciones,
        byte[] RowVersion
    ) : IRequest<CotizacionResponse>;
}