using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.ReporteHojaCorte;

/// <summary>
/// Parámetros literales de ges_sp_rpt_hoja_corte. A diferencia de la búsqueda, todos los filtros se combinan con AND:
///   p_anohco (4 caracteres) | p_nrohco (8) | p_codcli (5) | p_codobr (3) | p_codven (4) | p_codtec (5)
///   p_codest (código 01/04/13 o nombre) | p_tiphco (2) | un filtro vacío o de otra longitud no filtra.
///   p_opcion decide a qué fecha se aplica el rango p_fecini / p_fecfin (cualquiera puede omitirse; p_fecfin incluye el día completo):
///     1 = fecha de la hoja | 2 = fecha posible de despacho de viguetas del cliente | 3 = ídem bovedillas.
///   Con p_opcion distinto de 1, 2 o 3 no se filtra por fecha (igual que el SP).
/// </summary>
public sealed class ReporteHojaCorteQuery : IRequest<IReadOnlyList<ReporteHojaCorteRow>>
{
    public string? p_anohco { get; set; }
    public string? p_nrohco { get; set; }
    public DateTime? p_fecini { get; set; }
    public DateTime? p_fecfin { get; set; }
    public string? p_codcli { get; set; }
    public string? p_codobr { get; set; }
    public string? p_codven { get; set; }
    public string? p_codtec { get; set; }
    public string? p_codest { get; set; }
    public string? p_tiphco { get; set; }
    public string? p_opcion { get; set; }
}

/// <summary>Una fila por línea de detalle (una HC sin detalle sale en una fila con los datos de línea vacíos).</summary>
public sealed class ReporteHojaCorteRow
{
    // Cabecera
    public string Year { get; set; } = default!;
    public string Code { get; set; } = default!;
    public DateTime Date { get; set; }
    public string? ClienteCode { get; set; }
    public string ClientName { get; set; } = default!;
    public string? ObraCode { get; set; }
    public string WorkName { get; set; } = default!;
    public string Floor { get; set; } = default!;
    public string Sector { get; set; } = default!;
    public string TecnicoCode { get; set; } = default!;
    public string? TecnicoName { get; set; }
    public string VendedorCode { get; set; } = default!;
    public string? VendedorAbbreviation { get; set; }
    public string Address { get; set; } = default!;
    public DateTime? ViguetaDispatchDate { get; set; }
    public DateTime? BovedillaDispatchDate { get; set; }
    public DateTime? ClientViguetaDispatchDate { get; set; }
    public DateTime? ClientBovedillaDispatchDate { get; set; }
    public DateTime? StorageDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string StatusName { get; set; } = default!;
    public string TipoHojaCorteCode { get; set; } = default!;
    public string? TipoHojaCorteName { get; set; }
    public string? Observations { get; set; }
    public string Turn { get; set; } = default!;

    /// <summary>fn_busca_HC: "año  código  OT" de la hoja de origen (padre / copiada); vacío si no tiene.</summary>
    public string? ParentReference { get; set; }

    // Orden de trabajo vinculada
    public string? OrdenTrabajoCode { get; set; }
    public string? OrdenTrabajoApprovedBy { get; set; }
    public DateTime? OrdenTrabajoApprovedAt { get; set; }
    /// <summary>"Atendido 100%" si la OT está cerrada; en otro caso el estado de la OT.</summary>
    public string? OrdenTrabajoStatusName { get; set; }
    /// <summary>C = contado, R = crédito (de la aprobación; si no existe, de la forma de pago del cliente/obra).</summary>
    public string? PaymentCondition { get; set; }

    // Línea
    public string? ProductoTipoCode { get; set; }
    public string? ProductoTipoName { get; set; }
    public string? Panel { get; set; }
    public string? ProductoCode { get; set; }
    public string? ProductoName { get; set; }
    public decimal? Length { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? CablesSide1 { get; set; }
    public decimal? CablesSide2 { get; set; }
    /// <summary>1 por cada lado con más de 1 cable (SP: cables &gt; 1).</summary>
    public int? Sides { get; set; }
    /// <summary>ROUND(ISNULL(longitud,1) * cantidad, 2)</summary>
    public decimal? Total { get; set; }
    /// <summary>ROUND(cantidad * lados, 2)</summary>
    public decimal? QuantityPerSide { get; set; }
    public string? Lot1 { get; set; }
    public string? Lot2 { get; set; }
    public string? Lot3 { get; set; }
}
