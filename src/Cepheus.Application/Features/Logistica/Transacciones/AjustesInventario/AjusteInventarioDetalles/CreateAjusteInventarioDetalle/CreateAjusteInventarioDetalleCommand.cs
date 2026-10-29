using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.AjusteInventarioDetalles.CreateAjusteInventarioDetalle;

/// <summary>Agrega una línea a un ajuste Pendiente o en Entrega Parcial (PB: botón Adicionar del detalle).</summary>
public sealed record CreateAjusteInventarioDetalleCommand(
    string PlantaCode,
    string AjusteCode,
    string ArticuloCode,
    TipoAjusteInventario Tipo,
    decimal Cantidad) : IRequest<AjusteInventarioResponse>;

/// <summary>Cuerpo del POST; la planta y el ajuste vienen en la ruta.</summary>
public sealed record CreateAjusteInventarioDetalleRequest(
    string ArticuloCode,
    TipoAjusteInventario Tipo,
    decimal Cantidad);
