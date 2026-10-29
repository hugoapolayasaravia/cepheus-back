using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetNotaIngresoByCode;

/// <summary>Parámetros del GET legacy: Codigo_Pla + Codigo_NoI. Devuelve cabecera y detalle.</summary>
public sealed record GetNotaIngresoByCodeQuery(string Codigo_Pla, string Codigo_NoI) : IRequest<NotaIngresoResponse>;
