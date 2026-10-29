// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/GetCotizacionByCode/GetCotizacionByCodeQuery.cs
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.GetCotizacionByCode
{
    public record GetCotizacionByCodeQuery(string PlantaCode, string Code) : IRequest<CotizacionResponse>;
}