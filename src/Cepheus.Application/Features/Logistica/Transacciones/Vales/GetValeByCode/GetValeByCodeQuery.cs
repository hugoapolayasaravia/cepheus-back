using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.GetValeByCode;

/// <summary>Parámetros del GET legacy: Codigo_Pla + Codigo_Val. Devuelve cabecera y detalle.</summary>
public sealed record GetValeByCodeQuery(string Codigo_Pla, string Codigo_Val) : IRequest<ValeResponse>;
