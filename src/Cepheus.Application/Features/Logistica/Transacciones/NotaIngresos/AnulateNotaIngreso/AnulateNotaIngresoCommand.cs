using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.AnulateNotaIngreso;

/// <summary>
/// Anulación de una Nota de Ingreso. Revierte el stock y lo entregado en la OC
/// (Logi_sp_Anula_NotaIngreso / Logi_sp_Anula_NotaIngreso_NC).
/// </summary>
public sealed record AnulateNotaIngresoCommand(string PlantaCode, string Code) : IRequest<NotaIngresoResponse>;
