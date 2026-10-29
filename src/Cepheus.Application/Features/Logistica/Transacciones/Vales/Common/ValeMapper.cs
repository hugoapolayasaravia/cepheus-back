using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;

public static class ValeMapper
{
    public static string NombreTrabajador(string? nombres, string? apellidoPaterno)
        => $"{nombres} {apellidoPaterno}".Trim();

    public static ValeResponse Map(Vale v) => new()
    {
        PlantaCode = v.PlantaCode,
        Code = v.Code,
        TipoValeCode = v.TipoValeCode,
        TipoValeName = v.TipoVale.Name,
        Estado = v.Estado,
        FechaEntrega = v.FechaEntrega,
        FechaProceso = v.CreatedAt,
        SubCentroCostoCode = v.SubCentroCostoCode,
        SubCentroCostoName = v.SubCentroCosto.Name,
        CentroCostoCode = v.SubCentroCosto.CentroCostoCode,
        CentroCostoName = v.SubCentroCosto.CentroCosto?.Name,
        SubCentroEjecutorCode = v.SubCentroEjecutorCode,
        SubCentroEjecutorName = v.SubCentroEjecutor?.Name,
        TrabajadorCode = v.TrabajadorCode,
        TrabajadorName = NombreTrabajador(v.Trabajador.FirstNames, v.Trabajador.PaternalSurname),
        OrdenTrabajoCode = v.OrdenTrabajoCode,
        OrdenTrabajoDescription = v.OrdenTrabajo?.Description,
        EquipoCode = v.OrdenTrabajo?.EquipoCode,
        UnidadNegocioCode = v.UnidadNegocioCode,
        UnidadNegocioName = v.UnidadNegocio.Name,
        PlantaAfectadaCode = v.PlantaAfectadaCode,
        PlantaAfectadaName = v.PlantaAfectada.Name,
        Neto = v.Neto,
        Igv = v.Igv,
        Total = v.Total,
        AsientoContable = v.AsientoContable,
        AprobadoPor = v.AprobadoPor,
        FechaAprobacion = v.FechaAprobacion,
        ProcesadoPor = v.ProcesadoPor,
        FechaProcesado = v.FechaProcesado,
        AnuladoPor = v.AnuladoPor,
        FechaAnulacion = v.FechaAnulacion,
        Preparado = v.Preparado,
        CreatedBy = v.CreatedBy,
        Detalles = v.Detalles
            .OrderBy(d => d.ItemNumber)
            .Select(d => new ValeDetalleResponse
            {
                ItemNumber = d.ItemNumber,
                ArticuloCode = d.ArticuloCode,
                ArticuloName = d.Articulo.Name,
                UnidadMedidaCode = d.Articulo.UnidadMedidaCode,
                Cantidad = d.Cantidad,
                Precio = d.Precio,
                Total = d.Total,
                Estado = d.Estado,
                Propiedad01 = d.Propiedad01,
                MaterialOtFechaProceso = d.MaterialOtFechaProceso
            })
            .ToList()
    };

    public static ValeListadoResponse MapListado(Vale v) => new()
    {
        PlantaCode = v.PlantaCode,
        Code = v.Code,
        Estado = v.Estado,
        EstadoDescripcion = v.Estado.ToString(),
        FechaProceso = v.CreatedAt,
        Neto = v.Neto,
        Igv = v.Igv,
        Total = v.Total,
        SubCentroCostoCode = v.SubCentroCostoCode,
        SubCentroCostoName = v.SubCentroCosto.Name,
        CentroCostoName = v.SubCentroCosto.CentroCosto?.Name,
        TipoValeCode = v.TipoValeCode,
        TipoValeName = v.TipoVale.Name,
        Usuario = v.CreatedBy,
        OrdenTrabajoCode = v.OrdenTrabajoCode,
        OrdenTrabajoDescription = v.OrdenTrabajo?.Description,
        EquipoCode = v.OrdenTrabajo?.EquipoCode,
        TrabajadorCode = v.TrabajadorCode,
        TrabajadorName = NombreTrabajador(v.Trabajador.FirstNames, v.Trabajador.PaternalSurname),
        PlantaAfectadaCode = v.PlantaAfectadaCode,
        PlantaAfectadaName = v.PlantaAfectada.Name,
        AprobadoPor = v.AprobadoPor,
        FechaEntrega = v.FechaEntrega,
        UnidadNegocioCode = v.UnidadNegocioCode,
        Preparado = v.Preparado
    };
}
