using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.ValeDetalles.CreateValeDetalle;

/// <summary>Agrega una línea manual a un vale Pendiente (PB: botón Adicionar del detalle).</summary>
public sealed record CreateValeDetalleCommand(
    string PlantaCode,
    string ValeCode,
    string ArticuloCode,
    decimal Cantidad,
    string? Propiedad01) : IRequest<ValeResponse>;

/// <summary>Cuerpo del POST; la planta y el vale vienen en la ruta.</summary>
public sealed record CreateValeDetalleRequest(
    string ArticuloCode,
    decimal Cantidad,
    string? Propiedad01);
