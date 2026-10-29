using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.UpdateHojaCorte;

/// <summary>Cuerpo del PUT: los mismos datos de cabecera + fecha de la hoja y bloqueo. Año y código van en la ruta.</summary>
public sealed class UpdateHojaCorteRequest : HojaCorteHeaderRequest
{
    public DateTime Date { get; set; }

    /// <summary>false = bloquea la hoja (nadie puede modificarla).</summary>
    public bool IsModifiable { get; set; } = true;
}

public sealed record UpdateHojaCorteCommand(string Year, string Code, UpdateHojaCorteRequest Data)
    : IRequest<HojaCorteResponse>;
