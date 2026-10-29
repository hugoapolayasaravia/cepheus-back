using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.DevolverVale;

/// <summary>
/// Devuelve al almacén lo procesado: suma el stock (sin recalcular el promedio), pasa las líneas a Devuelto
/// y libera el consumo de la OT. Articulos vacío = todas las líneas procesadas; con lista = por ítem.
/// </summary>
public sealed record DevolverValeCommand(
    string PlantaCode,
    string Code,
    List<string>? Articulos) : IRequest<ValeResponse>;

/// <summary>Cuerpo del POST; planta y código vienen en la ruta.</summary>
public sealed record DevolverValeRequest(List<string>? Articulos);
