using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.AnulateVale;

/// <summary>
/// Anula el vale (solo Pendiente, Aprobado o Aprobación Provisional: todavía no movió stock).
/// Los materiales de la OT asignados al vale vuelven a pendiente.
/// </summary>
public sealed record AnulateValeCommand(string PlantaCode, string Code) : IRequest<ValeResponse>;
