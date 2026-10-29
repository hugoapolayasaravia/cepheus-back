using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.GetAjusteInventarioByCode;

/// <summary>Parámetros del GET legacy: Codigo_Pla + Codigo_Aju. Devuelve cabecera y detalle.</summary>
public sealed record GetAjusteInventarioByCodeQuery(string Codigo_Pla, string Codigo_Aju)
    : IRequest<AjusteInventarioResponse>;
