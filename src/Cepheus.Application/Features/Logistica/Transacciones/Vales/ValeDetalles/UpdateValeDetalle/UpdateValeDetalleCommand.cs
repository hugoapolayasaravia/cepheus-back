using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.ValeDetalles.UpdateValeDetalle;

/// <summary>
/// Modifica la cantidad (y el horómetro) de una línea pendiente de un vale Pendiente. La línea se identifica
/// por su artículo (la PK legacy); el precio es el que ya tenía la línea.
/// </summary>
public sealed record UpdateValeDetalleCommand(
    string PlantaCode,
    string ValeCode,
    string ArticuloCode,
    decimal Cantidad,
    string? Propiedad01) : IRequest<ValeResponse>;

/// <summary>Cuerpo del PUT; planta, vale y artículo vienen en la ruta.</summary>
public sealed record UpdateValeDetalleRequest(
    decimal Cantidad,
    string? Propiedad01);
