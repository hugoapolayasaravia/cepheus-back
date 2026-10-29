using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.ProcesarOrdenServicio;

/// <summary>
/// Procesa una Orden de Servicio Aprobada (botón "Procesar" del PowerBuilder): actualiza los costos de
/// stock de los artículos, registra los materiales de la Orden de Trabajo y deja la orden Procesada.
/// </summary>
public sealed record ProcesarOrdenServicioCommand(string PlantaCode, string Code) : IRequest<OrdenServicioResponse>;
