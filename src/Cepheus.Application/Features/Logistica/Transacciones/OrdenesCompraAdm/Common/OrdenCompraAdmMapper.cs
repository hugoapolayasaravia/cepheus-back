// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/Common/OrdenCompraAdministracionMapper.cs
using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common
{
    public static class OrdenCompraAdmMapper
    {
        public static OrdenCompraAdmResponse Map(OrdenCompraAdministracion o) => new()
        {
            Code = o.Code,
            ComprobantePagoCode = o.ComprobantePagoCode,
            EmitirDocumento = o.EmitirDocumento,
            FechaEntrega = o.FechaEntrega,
            ProveedorCode = o.ProveedorCode,
            ProveedorName = o.Proveedor?.LegalName,
            CompradorCode = o.CompradorCode,
            MonedaCode = o.MonedaCode,
            LugarEnvioCode = o.LugarEnvioCode,
            FormaPagoCode = o.FormaPagoCode,
            TramiteCode = o.TramiteCode,
            Observaciones1 = o.Observaciones1,
            Observaciones2 = o.Observaciones2,
            NotaCompraCode = o.NotaCompraCode,
            UnidadNegocioCode = o.UnidadNegocioCode,
            Motivo = o.Motivo.ToString(),
            Estado = o.Estado.ToString(),
            AprobadoPor = o.AprobadoPor,
            FechaAprobacion = o.FechaAprobacion,
            NetoCompra = o.NetoCompra,
            IgvCompra = o.IgvCompra,
            TotalCompra = o.TotalCompra,
            NoGravableCompra = o.NoGravableCompra,
            RentaCompra = o.RentaCompra,
            FonaviCompra = o.FonaviCompra,
            ServicioCompra = o.ServicioCompra,
            IgvExteriorCompra = o.IgvExteriorCompra,
            Detalles = o.Detalles
                .OrderBy(d => d.ItemNumber)
                .Select(d => new OrdenCompraAdmDetalleResponse
                {
                    ItemNumber = d.ItemNumber,
                    Glosa = d.Glosa,
                    UnidadMedidaCode = d.UnidadMedidaCode,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    Descuento = d.Descuento,
                    Total = d.Total,
                    SubCentroCostoCode = d.SubCentroCostoCode,
                    CantidadEntregada = d.CantidadEntregada
                }).ToList(),
            CreatedAt = o.CreatedAt,
            CreatedBy = o.CreatedBy,
            UpdatedAt = o.UpdatedAt,
            RowVersion = o.RowVersion
        };

        public static OrdenCompraAdmListItemResponse MapListItem(OrdenCompraAdministracion o) => new()
        {
            Code = o.Code,
            Estado = o.Estado.ToString(),
            ProveedorCode = o.ProveedorCode,
            ProveedorName = o.Proveedor?.LegalName,
            MonedaCode = o.MonedaCode,
            FechaEntrega = o.FechaEntrega,
            CreatedAt = o.CreatedAt,
            TramiteCode = o.TramiteCode,
            Motivo = o.Motivo.ToString(),
            TotalCompra = o.TotalCompra,
            CreatedBy = o.CreatedBy,
            AprobadoPor = o.AprobadoPor
        };
    }
}
