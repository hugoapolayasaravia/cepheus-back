using Cepheus.Domain.Logistica.Transacciones;
using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;

public static class OrdenServicioSalidaMapper
{
    public static OrdenServicioSalidaResumenResponse MapResumen(OrdenServicioSalida v) => new()
    {
        PlantaCode = v.PlantaCode,
        Code = v.Code,
        Estado = v.Estado,
        EstadoDescripcion = v.Estado.ToString(),
        FechaEntrega = v.FechaEntrega,
        TrabajadorCode = v.TrabajadorCode,
        TrabajadorName = NombreTrabajador(v.Trabajador),
        Neto = v.Neto,
        Igv = v.Igv,
        Total = v.Total,
        AsientoContable = v.AsientoContable,
        AprobadoPor = v.AprobadoPor,
        FechaAprobacion = v.FechaAprobacion,
        ProcesadoPor = v.ProcesadoPor,
        FechaProcesado = v.FechaProcesado
    };

    public static OrdenServicioSalidaResponse Map(OrdenServicioSalida v) => new()
    {
        PlantaCode = v.PlantaCode,
        PlantaName = v.Planta.Name,
        Code = v.Code,
        Estado = v.Estado,
        EstadoDescripcion = v.Estado.ToString(),
        FechaEntrega = v.FechaEntrega,
        TrabajadorCode = v.TrabajadorCode,
        TrabajadorName = NombreTrabajador(v.Trabajador),
        Neto = v.Neto,
        Igv = v.Igv,
        Total = v.Total,
        AsientoContable = v.AsientoContable,
        AprobadoPor = v.AprobadoPor,
        FechaAprobacion = v.FechaAprobacion,
        ProcesadoPor = v.ProcesadoPor,
        FechaProcesado = v.FechaProcesado,
        CreatedBy = v.CreatedBy,
        Detalles = v.Detalles
            .OrderBy(d => d.ItemNumber)
            .Select(d => new OrdenServicioSalidaDetalleResponse
            {
                ItemNumber = d.ItemNumber,
                ArticuloCode = d.ArticuloCode,
                ArticuloName = d.Articulo.Name,
                UnidadMedidaCode = d.Articulo.UnidadMedidaCode,
                Glosa = d.Glosa,
                Cantidad = d.Cantidad,
                Precio = d.Precio,
                Total = d.Total,
                Estado = d.Estado,
                AsientoContable = d.AsientoContable,
                Propiedad01 = d.Propiedad01,
                TipoValeCode = d.TipoValeCode,
                TipoValeName = d.TipoVale.Name,
                SubCentroCostoCode = d.SubCentroCostoCode,
                SubCentroCostoName = d.SubCentroCosto.Name,
                SubCentroEjecutorCode = d.SubCentroEjecutorCode,
                SubCentroEjecutorName = d.SubCentroEjecutor?.Name,
                OrdenTrabajoCode = d.OrdenTrabajoCode,
                PlantaAfectadaCode = d.PlantaAfectadaCode
            })
            .ToList()
    };

    public static OrdenServicioSalidaListadoResponse MapListado(OrdenServicioSalida v) => new()
    {
        PlantaCode = v.PlantaCode,
        Code = v.Code,
        Estado = v.Estado,
        EstadoDescripcion = v.Estado.ToString(),
        TrabajadorCode = v.TrabajadorCode,
        TrabajadorName = NombreTrabajador(v.Trabajador),
        FechaEntrega = v.FechaEntrega,
        Neto = v.Neto,
        Igv = v.Igv,
        Total = v.Total
    };

    public static string NombreTrabajador(Trabajador t)
        => string.Join(" ", new[] { t.PaternalSurname, t.MaternalSurname, t.FirstNames }
            .Where(s => !string.IsNullOrWhiteSpace(s)));
}
