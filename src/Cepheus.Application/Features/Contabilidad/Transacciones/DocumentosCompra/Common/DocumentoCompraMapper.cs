using Cepheus.Domain.Contabilidad.Transacciones;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common
{
    public static class DocumentoCompraMapper
    {
        public static DocumentoCompraResponse Map(DocumentoCompra d) => new()
        {
            ComprobantePagoCode = d.ComprobantePagoCode,
            NumeroDocumento = d.NumeroDocumento,
            ProveedorCode = d.ProveedorCode,
            ProveedorName = d.Proveedor?.LegalName,
            FechaEmision = d.FechaEmision,
            FechaRecepcion = d.FechaRecepcion,
            FechaDocumento = d.FechaDocumento,
            FechaVencimiento = d.FechaVencimiento,
            FormaPagoCode = d.FormaPagoCode,
            MonedaCode = d.MonedaCode,
            TipoCambioFecha = d.TipoCambioFecha,
            ParcialSol = d.ParcialSol,
            IgvSol = d.IgvSol,
            NoGravableSol = d.NoGravableSol,
            RentaSol = d.RentaSol,
            FonaviSol = d.FonaviSol,
            ServicioSol = d.ServicioSol,
            IgvExteriorSol = d.IgvExteriorSol,
            TotalSol = d.TotalSol,
            Glosa = d.Glosa,
            Origen = d.Origen.ToString(),
            Procedencia = d.Procedencia?.ToString(),
            Estado = d.Estado.ToString(),
            TipoBienCode = d.TipoBienCode,
            TipoOperacionCode = d.TipoOperacionCode,
            OperacionDetraccion = d.OperacionDetraccion,
            ConstanciaDetraccion = d.ConstanciaDetraccion,
            FechaConstanciaDetraccion = d.FechaConstanciaDetraccion,
            NumeroDetraccion = d.NumeroDetraccion,
            FechaEmisionDetraccion = d.FechaEmisionDetraccion,
            MontoDetraccionSol = d.MontoDetraccionSol,
            DocumentoOrigenComprobantePagoCode = d.DocumentoOrigenComprobantePagoCode,
            DocumentoOrigenNumero = d.DocumentoOrigenNumero,
            OrdenCompraAdministracionCode = d.OrdenCompraAdministracionCode,
            PagadoSol = d.PagadoSol,
            FechaCancelacion = d.FechaCancelacion,
            // PendienteSol es columna calculada en BD: en un documento recién creado/modificado aún no se refrescó.
            PendienteSol = d.TotalSol - d.PagadoSol,
            AsientoProvisionCode = d.AsientoProvisionCode,
            Detalles = d.Detalles
                .OrderBy(x => x.ItemNumber)
                .Select(x => new DocumentoCompraDetalleResponse
                {
                    ItemNumber = x.ItemNumber,
                    PlantaCode = x.PlantaCode,
                    CuentaContableCode = x.CuentaContableCode,
                    AnexoTipoCode = x.AnexoTipoCode,
                    AnexoCode = x.AnexoCode,
                    Detalle = x.Detalle,
                    Cantidad = x.Cantidad,
                    UnidadMedidaCode = x.UnidadMedidaCode,
                    PrecioSol = x.PrecioSol,
                    ParcialSol = x.ParcialSol,
                    NumeroGuiaVenta = x.NumeroGuiaVenta,
                    ProductoTipoCode = x.ProductoTipoCode,
                    ProductoCode = x.ProductoCode
                }).ToList(),
            CreatedAt = d.CreatedAt,
            CreatedBy = d.CreatedBy,
            UpdatedAt = d.UpdatedAt,
            RowVersion = d.RowVersion
        };

        public static DocumentoCompraListItemResponse MapListItem(DocumentoCompra d) => new()
        {
            ComprobantePagoCode = d.ComprobantePagoCode,
            NumeroDocumento = d.NumeroDocumento,
            ProveedorCode = d.ProveedorCode,
            ProveedorName = d.Proveedor?.LegalName,
            FechaEmision = d.FechaEmision,
            FechaProceso = d.CreatedAt,
            FormaPagoCode = d.FormaPagoCode,
            MonedaCode = d.MonedaCode,
            TipoCambioFecha = d.TipoCambioFecha,
            TotalSol = d.TotalSol,
            Estado = d.Estado.ToString(),
            AsientoProvisionCode = d.AsientoProvisionCode
        };
    }
}
