using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;

/// <summary>
/// Datos de cabecera que se capturan al crear y al modificar una Hoja de Corte (PB: dw_ges_reg_res_hoja_corte).
///
/// Cliente y obra:
///   - ClienteCode / ObraCode son opcionales (el PB permite Hoja de Corte sin cliente o sin obra).
///   - ClientName / WorkName / WorkAddress / WorkAddressUbigeoCode son el "snapshot" del documento. Si no se
///     envían y hay cliente/obra registrados, el servidor los toma del maestro (lo mismo que hace el PB al elegir
///     el cliente/obra con doble click).
///   - VendedorCode: si no se envía, se toma el vendedor de la obra (PB: itemchanged de codigo_obr).
///
/// No se reciben: Date (la fija el servidor), fechas de despacho de Viguetas/Bovedillas ("Datos posteriores"),
/// Estado, OT, Programado ni los datos de Despachador/Jefe de turno/Preparador (omitidos por el usuario).
/// </summary>
public class HojaCorteHeaderRequest
{
    public string TipoHojaCorteCode { get; set; } = default!;

    public string? ClienteCode { get; set; }
    public string? ClientName { get; set; }

    public string? ObraCode { get; set; }
    public string? WorkName { get; set; }
    public string? WorkAddress { get; set; }
    public string? WorkAddressUbigeoCode { get; set; }

    public string TecnicoCode { get; set; } = default!;
    public string? VendedorCode { get; set; }

    public string? Sector { get; set; }
    public string Floor { get; set; } = default!;

    /// <summary>Fecha posible de despacho de viguetas para el cliente (fecha y hora; obligatoria).</summary>
    public DateTime ClientViguetaDispatchDate { get; set; }

    /// <summary>Fecha posible de despacho de bovedillas para el cliente (fecha y hora; obligatoria).</summary>
    public DateTime ClientBovedillaDispatchDate { get; set; }

    /// <summary>Fecha de almacenaje (solo fecha; opcional).</summary>
    public DateTime? StorageDate { get; set; }

    public string? Observations { get; set; }

    public TurnoHojaCorte Turn { get; set; }

    /// <summary>Obligatorio solo cuando el tipo es Reemplazo (05).</summary>
    public MotivoHojaCorte? Motivo { get; set; }

    /// <summary>Obligatorio (Vigueta, Bovedilla o ViguetaYBovedilla).</summary>
    public TipoProductoHojaCorte? ProductType { get; set; }
}

/// <summary>Campos de una línea que no identifican al producto (comunes a alta y modificación).</summary>
public class HojaCorteDetalleFields
{
    /// <summary>Código de UnidadMedidaVenta. Si se omite se usa la unidad del producto.</summary>
    public string? UnitCode { get; set; }

    /// <summary>Cantidad ("B" en la grilla).</summary>
    public decimal Quantity { get; set; }

    /// <summary>Paño. Obligatorio en viguetas; se ignora en bovedillas.</summary>
    public string? Panel { get; set; }

    /// <summary>Longitud ("A" en la grilla). Obligatoria en viguetas; se ignora en bovedillas.</summary>
    public decimal? Length { get; set; }

    public decimal? CablesSide1 { get; set; }

    /// <summary>Solo se registra si hay cables en el lado 1 (PB: protección de cables2_hco).</summary>
    public decimal? CablesSide2 { get; set; }

    public string? Lot1 { get; set; }
    public string? Lot2 { get; set; }
    public string? Lot3 { get; set; }
}

/// <summary>Línea nueva: producto (tipo 16 vigueta / 18 bovedilla + código) y sus datos.</summary>
public sealed class HojaCorteDetalleRequest : HojaCorteDetalleFields
{
    public string ProductoTipoCode { get; set; } = default!;
    public string ProductoCode { get; set; } = default!;
}

/// <summary>Modificación de una línea. El producto no cambia (es parte de la llave): se elimina y se agrega otra.</summary>
public sealed class UpdateHojaCorteDetalleRequest : HojaCorteDetalleFields
{
}
