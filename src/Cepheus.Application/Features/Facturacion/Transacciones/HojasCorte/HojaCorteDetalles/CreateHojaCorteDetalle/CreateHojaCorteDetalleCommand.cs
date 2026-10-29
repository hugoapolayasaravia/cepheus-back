using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.HojaCorteDetalles.CreateHojaCorteDetalle;

/// <summary>Agrega una línea (vigueta 16 / bovedilla 18). El ítem lo asigna el servidor (correlativo por tipo).</summary>
public sealed record CreateHojaCorteDetalleCommand(string Year, string Code, HojaCorteDetalleRequest Data)
    : IRequest<HojaCorteResponse>;
