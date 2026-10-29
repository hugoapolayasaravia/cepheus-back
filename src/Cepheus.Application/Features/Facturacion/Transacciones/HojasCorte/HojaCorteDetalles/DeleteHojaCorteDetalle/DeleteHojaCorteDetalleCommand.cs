using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.HojaCorteDetalles.DeleteHojaCorteDetalle;

/// <summary>Elimina una línea. Los ítems restantes conservan su número (la llave de la línea no se renumera).</summary>
public sealed record DeleteHojaCorteDetalleCommand(
    string Year,
    string Code,
    string ProductoTipoCode,
    string ProductoCode,
    int ItemNumber) : IRequest<HojaCorteResponse>;
