using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.HojaCorteDetalles.UpdateHojaCorteDetalle;

/// <summary>Modifica una línea identificada por (tipo de producto, producto, ítem).</summary>
public sealed record UpdateHojaCorteDetalleCommand(
    string Year,
    string Code,
    string ProductoTipoCode,
    string ProductoCode,
    int ItemNumber,
    UpdateHojaCorteDetalleRequest Data) : IRequest<HojaCorteResponse>;
