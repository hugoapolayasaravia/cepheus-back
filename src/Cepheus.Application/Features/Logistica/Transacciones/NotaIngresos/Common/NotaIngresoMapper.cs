using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;

public static class NotaIngresoMapper
{
    public static NotaIngresoResponse Map(NotaIngreso n) => new()
    {
        PlantaCode = n.PlantaCode,
        Code = n.Code,
        Condicion = n.Condicion,
        Origen = n.Origen,
        OrdenCompraCode = n.OrdenCompraCode,
        ImportacionCode = n.ImportacionCode,
        ComprobantePagoCode = n.ComprobantePagoCode,
        ComprobantePagoName = n.ComprobantePago.Name,
        MotivoDevolucionCode = n.MotivoDevolucionCode,
        MotivoDevolucionName = n.MotivoDevolucion?.Name,
        NumeroDocumento = n.NumeroDocumento,
        NumeroGuia = n.NumeroGuia,
        NumeroReferencia = n.NumeroReferencia,
        ComprobantePagoReferenciaCode = n.ComprobantePagoReferenciaCode,
        ProveedorCode = n.ProveedorCode,
        ProveedorName = n.Proveedor.LegalName,
        MonedaCode = n.MonedaCode,
        FormaPagoCode = n.FormaPagoCode,
        FormaPagoName = n.FormaPago.Name,
        FechaRecepcion = n.FechaRecepcion,
        FechaEmision = n.FechaEmision,
        FechaProceso = n.FechaProceso,
        FechaDocumento = n.FechaDocumento,
        TipoCambio = n.TipoCambio,
        Estado = n.Estado,
        Igv = n.Igv,
        Total = n.Total,
        Monto = n.Monto,
        NoGravable = n.NoGravable,
        Renta = n.Renta,
        Fonavi = n.Fonavi,
        Servicio = n.Servicio,
        IgvExterior = n.IgvExterior,
        AsientoContable = n.AsientoContable,
        CodigoVale = n.CodigoVale,
        CreatedBy = n.CreatedBy,
        Detalles = n.Detalles
            .OrderBy(d => d.ItemNumber)
            .Select(d => new NotaIngresoDetalleResponse
            {
                ItemNumber = d.ItemNumber,
                ArticuloCode = d.ArticuloCode,
                ArticuloName = d.Articulo.Name,
                UnidadMedidaCode = d.Articulo.UnidadMedidaCode,
                PedidoCode = d.PedidoCode,
                GuiaCode = d.GuiaCode,
                Cantidad = d.Cantidad,
                Precio = d.Precio,
                Descuento = d.Descuento,
                Total = d.Total,
                Estado = d.Estado
            })
            .ToList()
    };

    public static NotaIngresoListadoResponse MapListado(NotaIngreso n) => new()
    {
        PlantaCode = n.PlantaCode,
        Code = n.Code,
        Estado = n.Estado,
        EstadoDescripcion = n.Estado.ToString(),
        ProveedorCode = n.ProveedorCode,
        ProveedorName = n.Proveedor.LegalName,
        MonedaCode = n.MonedaCode,
        FechaProceso = n.FechaProceso,
        TipoCambio = n.TipoCambio,
        Total = n.Total,
        FechaEmision = n.FechaEmision,
        NumeroDocumento = n.NumeroDocumento,
        ComprobantePagoCode = n.ComprobantePago.Code
    };
}
