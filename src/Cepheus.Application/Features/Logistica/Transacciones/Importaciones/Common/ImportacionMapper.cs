// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/Common/ImportacionMapper.cs
using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common
{
    public static class ImportacionMapper
    {
        public static ImportacionResponse Map(Importacion i) => new()
        {
            PlantaCode = i.PlantaCode,
            Code = i.Code,
            PesoNeto = i.PesoNeto,
            PesoBruto = i.PesoBruto,
            FechaPoliza = i.FechaPoliza,
            FechaEntrega = i.FechaEntrega,
            TipoCambio = i.TipoCambio,
            TotalFob = i.TotalFob,
            TotalFlete = i.TotalFlete,
            TotalSeguro = i.TotalSeguro,
            TotalAduana = i.TotalAduana,
            Advalorem = i.Advalorem,
            Sobretasa = i.Sobretasa,
            Igv = i.Igv,
            OtrosGastos = i.OtrosGastos,
            Estado = i.Estado.ToString(),
            Gastos = i.Gastos.Select(g => new ImportacionGastoResponse
            {
                ProveedorCode = g.ProveedorCode,
                NumeroDocumento = g.NumeroDocumento,
                ComprobantePagoCode = g.ComprobantePagoCode,
                MonedaCode = g.MonedaCode,
                Afecto = g.Afecto,
                FechaEmision = g.FechaEmision,
                TipoCambio = g.TipoCambio,
                NetoGasto = g.NetoGasto,
                NetoGastoInafecto = g.NetoGastoInafecto,
                Igv = g.Igv,
                IgvExterior = g.IgvExterior,
                Total = g.Total,
                Articulos = g.Articulos.Select(a => new ImportacionGastoArticuloResponse
                {
                    ArticuloCode = a.ArticuloCode,
                    ValorGasto = a.ValorGasto,
                    IgvGasto = a.IgvGasto,
                    IgvExtGasto = a.IgvExtGasto
                }).ToList(),
                RowVersion = g.RowVersion
            }).ToList(),
            Detalles = i.Detalles.Select(d => new ImportacionDetalleResponse
            {
                ProveedorCode = d.ProveedorCode,
                ArticuloCode = d.ArticuloCode,
                ComprobantePagoCode = d.ComprobantePagoCode,
                NumeroDocumento = d.NumeroDocumento,
                FechaEmision = d.FechaEmision,
                TipoCambio = d.TipoCambio,
                Cantidad = d.Cantidad,
                ValorFob = d.ValorFob,
                Flete = d.Flete,
                Seguro = d.Seguro,
                ValorAduana = d.ValorAduana,
                PorcentajeDet = d.PorcentajeDet,
                ValorDet = d.ValorDet,
                AdvaloremDet = d.AdvaloremDet,
                SobretasaDet = d.SobretasaDet,
                IgvDet = d.IgvDet,
                OtrosGastosDet = d.OtrosGastosDet,
                Estado = d.Estado.ToString(),
                RowVersion = d.RowVersion
            }).ToList(),
            CreatedAt = i.CreatedAt,
            CreatedBy = i.CreatedBy,
            UpdatedAt = i.UpdatedAt,
            RowVersion = i.RowVersion
        };
    }
}
