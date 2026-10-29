using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.ProcesarVale;

/// <summary>
/// Procesa el vale: descuenta el stock, pasa las líneas a Procesado y registra el consumo en la OT.
/// Articulos vacío = todas las líneas pendientes; con lista = proceso por ítem (el vale queda en
/// Entrega Parcial si quedan líneas pendientes).
/// </summary>
public sealed record ProcesarValeCommand(
    string PlantaCode,
    string Code,
    List<string>? Articulos) : IRequest<ValeResponse>;

/// <summary>Cuerpo del POST; planta y código vienen en la ruta.</summary>
public sealed record ProcesarValeRequest(List<string>? Articulos);
