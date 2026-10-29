using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.UpdateVale;

/// <summary>
/// Modificación de la cabecera (solo en estado Pendiente). La planta, el tipo de vale y la Orden de Trabajo
/// no se pueden cambiar. Si el vale tiene OT, responsable, subcentro y ejecutor siguen a la OT.
/// </summary>
public sealed record UpdateValeCommand(
    string PlantaCode,
    string Code,
    DateTime FechaEntrega,
    string? SubCentroCostoCode,
    string? SubCentroEjecutorCode,
    string? TrabajadorCode,
    string UnidadNegocioCode,
    string? PlantaAfectadaCode) : IRequest<ValeResponse>;

/// <summary>Cuerpo del PUT; la planta y el código vienen en la ruta.</summary>
public sealed record UpdateValeRequest(
    DateTime FechaEntrega,
    string? SubCentroCostoCode,
    string? SubCentroEjecutorCode,
    string? TrabajadorCode,
    string UnidadNegocioCode,
    string? PlantaAfectadaCode);
