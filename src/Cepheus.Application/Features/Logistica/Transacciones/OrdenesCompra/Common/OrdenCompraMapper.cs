// Cepheus.Application/Features/Logistica/Transacciones/OrdenesCompra/Common/OrdenCompraMapper.cs
using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common
{
    public static class OrdenCompraMapper
    {
        public static OrdenCompraResponse Map(OrdenCompra o) => new()
        {
            PlantaCode = o.PlantaCode,
            Code = o.Code,
            TipoCompraCode = o.TipoCompraCode,
            ComprobantePagoCode = o.ComprobantePagoCode,
            FechaEntrega = o.FechaEntrega,
            ProveedorCode = o.ProveedorCode,
            CompradorCode = o.CompradorCode,
            MonedaCode = o.MonedaCode,
            LugarEnvioCode = o.LugarEnvioCode,
            FormaPagoCode = o.FormaPagoCode,
            TramiteCode = o.TramiteCode,
            Observaciones1 = o.Observaciones1,
            Observaciones2 = o.Observaciones2,
            NotaCompraCode = o.NotaCompraCode,
            UnidadNegocioCode = o.UnidadNegocioCode,
            EnviarCorreoProveedor = o.EnviarCorreoProveedor,
            Estado = o.Estado.ToString(),
            AprobadoPor = o.AprobadoPor,
            FechaAprobacion = o.FechaAprobacion,
            MotivoRetraso = o.MotivoRetraso,
            NetoCompra = o.NetoCompra,
            IgvCompra = o.IgvCompra,
            TotalCompra = o.TotalCompra,
            NoGravableCompra = o.NoGravableCompra,
            RentaCompra = o.RentaCompra,
            FonaviCompra = o.FonaviCompra,
            ServicioCompra = o.ServicioCompra,
            IgvExteriorCompra = o.IgvExteriorCompra,
            Detalles = o.Detalles.Select(d => new OrdenCompraDetalleResponse
            {
                ArticuloCode = d.ArticuloCode,
                ItemNumber = d.ItemNumber,
                CantidadArticulo = d.CantidadArticulo,
                PrecioArticulo = d.PrecioArticulo,
                DescuentoArticulo = d.DescuentoArticulo,
                TotalArticulo = d.TotalArticulo,
                CantidadEntregada = d.CantidadEntregada,
                SubCentroCostoCode = d.SubCentroCostoCode,
                Origenes = d.Origenes.Select(x => new OrdenCompraPedidoOrigenResponse
                {
                    PedidoCode = x.PedidoCode,
                    PedidoItemNumber = x.PedidoItemNumber,
                    CantidadTomada = x.CantidadTomada
                }).ToList()
            }).ToList(),
            CreatedAt = o.CreatedAt,
            CreatedBy = o.CreatedBy,
            UpdatedAt = o.UpdatedAt,
            RowVersion = o.RowVersion
        };
    }
}