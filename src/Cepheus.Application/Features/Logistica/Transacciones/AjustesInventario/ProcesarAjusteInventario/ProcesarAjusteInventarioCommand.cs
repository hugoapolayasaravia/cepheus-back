using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.ProcesarAjusteInventario;

/// <summary>
/// Procesa el ajuste: los sobrantes suman stock, los faltantes lo restan (sin recalcular el promedio) y las
/// líneas pasan a Procesado. Articulos vacío = todas las líneas pendientes; con lista = proceso por ítem
/// (el ajuste queda en Entrega Parcial si quedan líneas pendientes).
/// </summary>
public sealed record ProcesarAjusteInventarioCommand(
    string PlantaCode,
    string Code,
    List<string>? Articulos) : IRequest<AjusteInventarioResponse>;

/// <summary>Cuerpo del POST; planta y código vienen en la ruta.</summary>
public sealed record ProcesarAjusteInventarioRequest(List<string>? Articulos);
