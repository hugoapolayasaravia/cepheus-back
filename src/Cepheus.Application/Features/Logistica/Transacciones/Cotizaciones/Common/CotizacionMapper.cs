// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/Common/CotizacionMapper.cs
using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common
{
    public static class CotizacionMapper
    {
        public static CotizacionResponse Map(Cotizacion c) => new()
        {
            PlantaCode = c.PlantaCode,
            Code = c.Code,
            FechaLimite = c.FechaLimite,
            Estado = c.Estado.ToString(),
            Observaciones = c.Observaciones,
            OriginalCode = c.OriginalCode,
            FechaCierre = c.FechaCierre,
            Detalles = c.Detalles.Select(d => new CotizacionDetalleResponse
            {
                ArticuloCode = d.ArticuloCode,
                ItemNumber = d.ItemNumber,
                CantidadArticulo = d.CantidadArticulo,
                Origenes = d.Origenes.Select(o => new CotizacionPedidoOrigenResponse
                {
                    PedidoCode = o.PedidoCode,
                    PedidoItemNumber = o.PedidoItemNumber,
                    CantidadTomada = o.CantidadTomada
                }).ToList()
            }).ToList(),
            Proveedores = c.Proveedores.Select(p => new CotizacionProveedorResponse
            {
                ProveedorCode = p.ProveedorCode,
                MonedaCode = p.MonedaCode,
                NetoCotizacion = p.NetoCotizacion,
                IgvCotizacion = p.IgvCotizacion,
                TotalCotizacion = p.TotalCotizacion,
                Observaciones = p.Observaciones,
                Estado = p.Estado.ToString(),
                FechaRespuesta = p.FechaRespuesta,
                Detalles = p.Detalles.Select(l => new CotizacionProveedorDetalleResponse
                {
                    ArticuloCode = l.ArticuloCode,
                    CantidadArticulo = l.CantidadArticulo,
                    PrecioArticulo = l.PrecioArticulo,
                    DescuentoArticulo = l.DescuentoArticulo,
                    TotalLinea = l.TotalLinea
                }).ToList(),
                RowVersion = p.RowVersion
            }).ToList(),
            CreatedAt = c.CreatedAt,
            CreatedBy = c.CreatedBy,
            UpdatedAt = c.UpdatedAt,
            RowVersion = c.RowVersion
        };
    }
}