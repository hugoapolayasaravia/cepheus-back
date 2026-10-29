using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.AnulateAjusteInventario;

/// <summary>Anula el ajuste (solo Pendiente: todavía no movió stock).</summary>
public sealed record AnulateAjusteInventarioCommand(string PlantaCode, string Code) : IRequest<AjusteInventarioResponse>;
