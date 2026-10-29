using Cepheus.Domain.Facturacion.Transacciones;
using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;

public static class HojaCorteMapper
{
    public static HojaCorteResponse Map(HojaCorte h)
    {
        var viguetas = h.Detalles
            .Where(d => d.ProductoTipoCode == HojaCorteProductoTipos.Vigueta)
            .OrderBy(d => d.ItemNumber)
            .ThenBy(d => d.ProductoCode, StringComparer.Ordinal)
            .ToList();

        var bovedillas = h.Detalles
            .Where(d => d.ProductoTipoCode == HojaCorteProductoTipos.Bovedilla)
            .OrderBy(d => d.ItemNumber)
            .ThenBy(d => d.ProductoCode, StringComparer.Ordinal)
            .ToList();

        return new HojaCorteResponse
        {
            Year = h.Year,
            Code = h.Code,
            TipoHojaCorteCode = h.TipoHojaCorteCode,
            TipoHojaCorteName = h.TipoHojaCorte.Name,
            Status = h.Status,
            StatusDescription = h.Status.ToString(),
            ClienteCode = h.ClienteCode,
            ClientName = h.ClientName,
            ObraCode = h.ObraCode,
            WorkName = h.WorkName,
            WorkAddress = h.WorkAddress,
            WorkAddressUbigeoCode = h.WorkAddressUbigeoCode,
            TecnicoCode = h.TecnicoCode,
            TecnicoName = NombreTrabajador(h.Tecnico.Trabajador),
            VendedorCode = h.VendedorCode,
            VendedorName = h.Vendedor.Name,
            Sector = h.Sector,
            Floor = h.Floor,
            Date = h.Date,
            ViguetaDispatchDate = h.ViguetaDispatchDate,
            BovedillaDispatchDate = h.BovedillaDispatchDate,
            ClientViguetaDispatchDate = h.ClientViguetaDispatchDate,
            ClientBovedillaDispatchDate = h.ClientBovedillaDispatchDate,
            StorageDate = h.StorageDate,
            OrdenTrabajoCode = h.OrdenTrabajoCode,
            Observations = h.Observations,
            IsScheduled = h.IsScheduled,
            IsModifiable = h.IsModifiable,
            ParentYear = h.ParentYear,
            ParentCode = h.ParentCode,
            Turn = h.Turn,
            Motivo = h.Motivo,
            ProductType = h.ProductType,
            ProcessDate = h.CreatedAt,
            CreatedBy = h.CreatedBy,
            Viguetas = viguetas.Select(MapDetalle).ToList(),
            Bovedillas = bovedillas.Select(MapDetalle).ToList(),
            TotalesViguetas = Totales(viguetas),
            TotalesBovedillas = Totales(bovedillas),
            Advertencias = viguetas.SelectMany(HojaCorteRules.LineWarnings).ToList()
        };
    }

    public static HojaCorteListadoResponse MapListado(HojaCorte h) => new()
    {
        Year = h.Year,
        Code = h.Code,
        Date = h.Date,
        ClienteCode = h.ClienteCode,
        ObraCode = h.ObraCode,
        ClientName = h.ClientName,
        TecnicoCode = h.TecnicoCode,
        TecnicoName = NombreTrabajador(h.Tecnico.Trabajador),
        VendedorCode = h.VendedorCode,
        VendedorName = h.Vendedor.Name,
        Status = h.Status,
        StatusDescription = h.Status.ToString(),
        TipoHojaCorteCode = h.TipoHojaCorteCode,
        TipoHojaCorteName = h.TipoHojaCorte.Name
    };

    public static HojaCorteDetalleResponse MapDetalle(HojaCorteDetalle d) => new()
    {
        ItemNumber = d.ItemNumber,
        ProductoTipoCode = d.ProductoTipoCode,
        ProductoCode = d.ProductoCode,
        ProductoName = d.Producto.Name,
        UnitCode = d.UnitCode,
        UnitName = d.Unit.Name,
        Quantity = d.Quantity,
        Panel = d.Panel,
        Length = d.Length,
        CablesSide1 = d.CablesSide1,
        CablesSide2 = d.CablesSide2,
        Sides = d.Sides,
        TotalLength = d.TotalLength,
        TotalCables = d.TotalCables,
        Cuts = d.Cuts,
        Lot1 = d.Lot1,
        Lot2 = d.Lot2,
        Lot3 = d.Lot3
    };

    private static HojaCorteTotalesResponse Totales(List<HojaCorteDetalle> lineas) => new()
    {
        Length = lineas.Sum(d => d.Length ?? 0m),
        Quantity = lineas.Sum(d => d.Quantity),
        TotalLength = lineas.Sum(d => d.TotalLength),
        CablesSide1 = lineas.Sum(d => d.CablesSide1 ?? 0m),
        CablesSide2 = lineas.Sum(d => d.CablesSide2 ?? 0m),
        TotalCables = lineas.Sum(d => d.TotalCables),
        Cuts = lineas.Sum(d => d.Cuts)
    };

    /// <summary>Apellido paterno, materno y nombres (mismo criterio que OrdenServicioSalidaMapper).</summary>
    public static string NombreTrabajador(Trabajador t)
        => string.Join(" ", new[] { t.PaternalSurname, t.MaternalSurname, t.FirstNames }
            .Where(s => !string.IsNullOrWhiteSpace(s)));
}
