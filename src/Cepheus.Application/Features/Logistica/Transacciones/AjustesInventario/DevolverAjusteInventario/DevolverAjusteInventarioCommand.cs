using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.DevolverAjusteInventario;

/// <summary>
/// Revierte lo procesado: invierte el efecto de cada línea en el stock (sobrante resta, faltante suma, sin
/// recalcular el promedio) y las pasa a Devuelto. Articulos vacío = todas las líneas procesadas;
/// con lista = devolución por ítem.
/// </summary>
public sealed record DevolverAjusteInventarioCommand(
    string PlantaCode,
    string Code,
    List<string>? Articulos) : IRequest<AjusteInventarioResponse>;

/// <summary>Cuerpo del POST; planta y código vienen en la ruta.</summary>
public sealed record DevolverAjusteInventarioRequest(List<string>? Articulos);
