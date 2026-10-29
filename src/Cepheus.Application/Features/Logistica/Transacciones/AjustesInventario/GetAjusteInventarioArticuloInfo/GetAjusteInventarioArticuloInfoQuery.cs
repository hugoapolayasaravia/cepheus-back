using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.GetAjusteInventarioArticuloInfo;

/// <summary>Stock, reservado por ajustes y precio promedio de un artículo para armar una línea (PB: ue_saldos).</summary>
public sealed record GetAjusteInventarioArticuloInfoQuery(string PlantaCode, string ArticuloCode)
    : IRequest<AjusteInventarioArticuloInfoResponse>;
