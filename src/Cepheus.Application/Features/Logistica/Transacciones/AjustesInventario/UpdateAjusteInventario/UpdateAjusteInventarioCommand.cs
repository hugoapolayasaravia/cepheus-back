using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.UpdateAjusteInventario;

/// <summary>
/// Modificación de la cabecera (fecha de entrega y observación), en Pendiente o Entrega Parcial.
/// La planta no se puede cambiar.
/// </summary>
public sealed record UpdateAjusteInventarioCommand(
    string PlantaCode,
    string Code,
    DateTime FechaEntrega,
    string? Observacion) : IRequest<AjusteInventarioResponse>;

/// <summary>Cuerpo del PUT; la planta y el código vienen en la ruta.</summary>
public sealed record UpdateAjusteInventarioRequest(
    DateTime FechaEntrega,
    string? Observacion);
