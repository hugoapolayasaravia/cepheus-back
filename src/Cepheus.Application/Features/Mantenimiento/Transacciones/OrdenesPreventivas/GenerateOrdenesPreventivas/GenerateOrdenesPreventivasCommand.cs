using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesPreventivas.GenerateOrdenesPreventivas;

/// <summary>
/// Genera las Órdenes de Trabajo preventivas que "tocan" en el rango (legacy ue_set_btn_grb_generar_ot).
/// El servidor calcula el calendario de equipos y crea una OT por cada plan y día con TocaGenerarOrden, más un
/// vale de salida con los materiales del plan. Todo en una sola transacción.
/// </summary>
/// <param name="PlantaCode">Planta de los planes (el legacy trabaja siempre sobre una planta).</param>
/// <param name="EquipoCode">Coincidencia parcial del código de equipo; null = todos.</param>
/// <param name="PlanCodes">Solo estos planes; null o vacío = todos los del filtro.</param>
public sealed record GenerateOrdenesPreventivasCommand(
    string PlantaCode,
    string? EquipoCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    IReadOnlyList<string>? PlanCodes) : IRequest<GenerateOrdenesPreventivasResponse>;
