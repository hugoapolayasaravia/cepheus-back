using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.GetValeArticuloInfo;

/// <summary>
/// Datos del artículo para armar una línea de vale: stock, reservado, disponible, precio promedio y horómetro
/// anterior (PB: ue_saldos, ue_horometro y ue_busqueda de dw_6). SubCentroCostoCode y FechaEntrega solo se usan
/// para el horómetro anterior; TipoValeCode, para indicar si el artículo está permitido.
/// </summary>
public sealed record GetValeArticuloInfoQuery(
    string PlantaCode,
    string ArticuloCode,
    string? TipoValeCode,
    string? SubCentroCostoCode,
    DateTime? FechaEntrega) : IRequest<ValeArticuloInfoResponse>;
