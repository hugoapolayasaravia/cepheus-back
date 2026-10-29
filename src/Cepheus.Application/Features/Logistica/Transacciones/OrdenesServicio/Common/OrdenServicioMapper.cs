using Cepheus.Domain.Logistica.Transacciones;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;

public static class OrdenServicioMapper
{
    public static OrdenServicioResponse Map(OrdenServicio o) => new()
    {
        PlantaCode = o.PlantaCode,
        PlantaName = o.Planta.Name,
        Code = o.Code,
        Estado = o.Estado,
        EstadoDescripcion = o.Estado.ToString(),
        ComprobantePagoCode = o.ComprobantePagoCode,
        ComprobantePagoName = o.ComprobantePago.Name,
        NumeroDocumento = o.NumeroDocumento,
        ProveedorCode = o.ProveedorCode,
        ProveedorName = o.Proveedor.LegalName,
        MonedaCode = o.MonedaCode,
        FormaPagoCode = o.FormaPagoCode,
        FormaPagoName = o.FormaPago.Name,
        FechaRecepcion = o.FechaRecepcion,
        FechaEmision = o.FechaEmision,
        TipoCambio = o.TipoCambio,
        Igv = o.Igv,
        Total = o.Total,
        Monto = o.Monto,
        NoGravable = o.NoGravable,
        Renta = o.Renta,
        Fonavi = o.Fonavi,
        Servicio = o.Servicio,
        IgvExterior = o.IgvExterior,
        AsientoContable = o.AsientoContable,
        AprobadoPor = o.AprobadoPor,
        FechaAprobacion = o.FechaAprobacion,
        ProcesadoPor = o.ProcesadoPor,
        FechaProcesado = o.ValeSalida.FechaProcesado,
        CompradorCode = o.CompradorCode,
        CompradorName = o.Comprador.Name,
        LugarEnvioCode = o.LugarEnvioCode,
        LugarEnvioName = o.LugarEnvio.Name,
        TramiteCode = o.TramiteCode,
        TramiteName = o.Tramite.Name,
        NotaCompraCode = o.NotaCompraCode,
        NotaCompraName = o.NotaCompra?.Name,
        UnidadNegocioCode = o.UnidadNegocioCode,
        UnidadNegocioName = o.UnidadNegocio.Name,
        ValeSalida = OrdenServicioSalidaMapper.MapResumen(o.ValeSalida),
        Observaciones1 = o.Observaciones1,
        Observaciones2 = o.Observaciones2,
        CreatedBy = o.CreatedBy,
        Detalles = o.Detalles
            .OrderBy(d => d.ItemNumber)
            .Select(d => new OrdenServicioDetalleResponse
            {
                ItemNumber = d.ItemNumber,
                ArticuloCode = d.ArticuloCode,
                ArticuloName = d.Articulo.Name,
                UnidadMedidaCode = d.Articulo.UnidadMedidaCode,
                Glosa = d.Glosa,
                Cantidad = d.Cantidad,
                Precio = d.Precio,
                Descuento = d.Descuento,
                Total = d.Total,
                Estado = d.Estado,
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

    public static OrdenServicioListadoResponse MapListado(OrdenServicio o) => new()
    {
        PlantaCode = o.PlantaCode,
        Code = o.Code,
        Estado = o.Estado,
        EstadoDescripcion = o.Estado.ToString(),
        ProveedorCode = o.ProveedorCode,
        ProveedorName = o.Proveedor.LegalName,
        MonedaCode = o.MonedaCode,
        TipoCambio = o.TipoCambio,
        Total = o.Total,
        FechaEmision = o.FechaEmision,
        NumeroDocumento = o.NumeroDocumento,
        ComprobantePagoCode = o.ComprobantePagoCode,
        ComprobantePagoName = o.ComprobantePago.Name
    };
}
