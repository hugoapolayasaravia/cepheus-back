using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;

public static class AjusteInventarioMapper
{
    public static AjusteInventarioResponse Map(AjusteInventario a) => new()
    {
        PlantaCode = a.PlantaCode,
        PlantaName = a.Planta.Name,
        Code = a.Code,
        Estado = a.Estado,
        Observacion = a.Observacion,
        FechaEntrega = a.FechaEntrega,
        FechaProceso = a.CreatedAt,
        Neto = a.Neto,
        Igv = a.Igv,
        Total = a.Total,
        AsientoContable = a.AsientoContable,
        CreatedBy = a.CreatedBy,
        Detalles = a.Detalles
            .OrderBy(d => d.ItemNumber)
            .Select(d => new AjusteInventarioDetalleResponse
            {
                ItemNumber = d.ItemNumber,
                ArticuloCode = d.ArticuloCode,
                ArticuloName = d.Articulo.Name,
                UnidadMedidaCode = d.Articulo.UnidadMedidaCode,
                Tipo = d.Tipo,
                Cantidad = d.Cantidad,
                Precio = d.Precio,
                Total = d.Total,
                Estado = d.Estado
            })
            .ToList()
    };

    public static AjusteInventarioListadoResponse MapListado(AjusteInventario a) => new()
    {
        PlantaCode = a.PlantaCode,
        Code = a.Code,
        Estado = a.Estado,
        EstadoDescripcion = a.Estado.ToString(),
        FechaProceso = a.CreatedAt,
        Neto = a.Neto,
        Usuario = a.CreatedBy
    };
}
