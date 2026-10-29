using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;

public sealed class HojaCorteDetalleResponse
{
    public int ItemNumber { get; set; }
    public string ProductoTipoCode { get; set; } = default!;
    public string ProductoCode { get; set; } = default!;
    public string ProductoName { get; set; } = default!;
    public string UnitCode { get; set; } = default!;
    public string UnitName { get; set; } = default!;
    public decimal Quantity { get; set; }
    public string? Panel { get; set; }
    public decimal? Length { get; set; }
    public decimal? CablesSide1 { get; set; }
    public decimal? CablesSide2 { get; set; }

    /// <summary>Lados = cantidad de lados con cables (0, 1 o 2).</summary>
    public int Sides { get; set; }

    /// <summary>Total A * B = Longitud x Cantidad.</summary>
    public decimal TotalLength { get; set; }

    /// <summary>Total Cables = Cables lado 1 + Cables lado 2.</summary>
    public decimal TotalCables { get; set; }

    /// <summary>Cortes B / 12.</summary>
    public decimal Cuts { get; set; }

    public string? Lot1 { get; set; }
    public string? Lot2 { get; set; }
    public string? Lot3 { get; set; }
}

/// <summary>Fila "Totales" de la grilla (PB: summary del DataWindow).</summary>
public sealed class HojaCorteTotalesResponse
{
    public decimal Length { get; set; }
    public decimal Quantity { get; set; }
    public decimal TotalLength { get; set; }
    public decimal CablesSide1 { get; set; }
    public decimal CablesSide2 { get; set; }
    public decimal TotalCables { get; set; }
    public decimal Cuts { get; set; }
}

public sealed class HojaCorteResponse
{
    public string Year { get; set; } = default!;
    public string Code { get; set; } = default!;

    public string TipoHojaCorteCode { get; set; } = default!;
    public string TipoHojaCorteName { get; set; } = default!;

    public EstadoHojaCorte Status { get; set; }
    public string StatusDescription { get; set; } = default!;

    public string? ClienteCode { get; set; }
    public string ClientName { get; set; } = default!;

    public string? ObraCode { get; set; }
    public string WorkName { get; set; } = default!;
    public string WorkAddress { get; set; } = default!;
    public string WorkAddressUbigeoCode { get; set; } = default!;

    public string TecnicoCode { get; set; } = default!;
    public string TecnicoName { get; set; } = default!;

    public string VendedorCode { get; set; } = default!;
    public string VendedorName { get; set; } = default!;

    public string Sector { get; set; } = default!;
    public string Floor { get; set; } = default!;

    public DateTime Date { get; set; }
    public DateTime? ViguetaDispatchDate { get; set; }
    public DateTime? BovedillaDispatchDate { get; set; }
    public DateTime? ClientViguetaDispatchDate { get; set; }
    public DateTime? ClientBovedillaDispatchDate { get; set; }
    public DateTime? StorageDate { get; set; }

    public string? OrdenTrabajoCode { get; set; }
    public string? Observations { get; set; }

    public bool IsScheduled { get; set; }
    public bool IsModifiable { get; set; }

    /// <summary>Hoja de Corte padre (Adicional/Reemplazo) o de origen (copia).</summary>
    public string? ParentYear { get; set; }
    public string? ParentCode { get; set; }

    public TurnoHojaCorte Turn { get; set; }
    public MotivoHojaCorte? Motivo { get; set; }
    public TipoProductoHojaCorte? ProductType { get; set; }

    /// <summary>Fecha y hora de registro (auditoría; reemplaza a fecproceso_hco).</summary>
    public DateTime ProcessDate { get; set; }

    /// <summary>Usuario que la registró (auditoría; reemplaza a Login_usr).</summary>
    public string? CreatedBy { get; set; }

    public List<HojaCorteDetalleResponse> Viguetas { get; set; } = new();
    public List<HojaCorteDetalleResponse> Bovedillas { get; set; } = new();

    public HojaCorteTotalesResponse TotalesViguetas { get; set; } = new();
    public HojaCorteTotalesResponse TotalesBovedillas { get; set; } = new();

    /// <summary>
    /// Advertencias no bloqueantes del PB (longitud mayor al máximo del producto, longitud irregular).
    /// </summary>
    public List<string> Advertencias { get; set; } = new();
}

/// <summary>Fila del listado paginado. Campos de salida de ges_sp_bus_hoja_corte.</summary>
public sealed class HojaCorteListadoResponse
{
    public string Year { get; set; } = default!;
    public string Code { get; set; } = default!;
    public DateTime Date { get; set; }
    public string? ClienteCode { get; set; }
    public string? ObraCode { get; set; }
    public string ClientName { get; set; } = default!;
    public string TecnicoCode { get; set; } = default!;
    public string TecnicoName { get; set; } = default!;
    public string VendedorCode { get; set; } = default!;
    public string VendedorName { get; set; } = default!;
    public EstadoHojaCorte Status { get; set; }
    public string StatusDescription { get; set; } = default!;
    public string TipoHojaCorteCode { get; set; } = default!;
    public string TipoHojaCorteName { get; set; } = default!;
}
