using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.CreateHojaCorte;

/// <summary>
/// Registro de una Hoja de Corte (nace Pendiente, con la fecha y el correlativo del servidor).
/// Adicional (03) y Reemplazo (05) requieren ParentYear/ParentCode. Las líneas son opcionales al crear.
/// </summary>
public sealed class CreateHojaCorteCommand : HojaCorteHeaderRequest, IRequest<HojaCorteResponse>
{
    public string? ParentYear { get; set; }
    public string? ParentCode { get; set; }
    public List<HojaCorteDetalleRequest> Detalles { get; set; } = new();
}
