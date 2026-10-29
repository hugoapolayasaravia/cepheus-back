using Cepheus.Domain.Facturacion.Transacciones;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.Common
{
    /// <summary>
    /// Mapeo compartido Cotizacion -> CotizacionResponse, usado por todos los
    /// handlers que devuelven la cabecera (Create/Update/Approve/Anular/
    /// ChangeEstado/GetByCode), mismo criterio que
    /// CreateOrdenTrabajoCommandHandler.Map en Mantenimiento.
    /// </summary>
    internal static class CotizacionMapper
    {
        public static CotizacionResponse Map(Cotizacion c) => new()
        {
            NegocioCode = c.NegocioCode,
            Year = c.Year,
            Month = c.Month,
            Code = c.Code,
            VendedorCode = c.VendedorCode,
            Date = c.Date,
            CurrencyCode = c.CurrencyCode,
            FormaPagoVentaCode = c.FormaPagoVentaCode,
            TecnicoCode = c.TecnicoCode,
            AppliesIgv = c.AppliesIgv,
            Discount = c.Discount,
            GlobalVolume = c.GlobalVolume,
            IsEditable = c.IsEditable,
            Type = c.Type.ToString(),
            MetradoCalculationSystem = c.MetradoCalculationSystem?.ToString(),
            WorkDurationMonths = c.WorkDurationMonths,
            ClienteCode = c.ClienteCode,
            Ruc = c.Ruc,
            ClientName = c.ClientName,
            ClientAddress = c.ClientAddress,
            ClientAddressUbigeoCode = c.ClientAddressUbigeoCode,
            ObraCode = c.ObraCode,
            WorkName = c.WorkName,
            ProjectStatus = c.ProjectStatus.ToString(),
            WorkAddressUbigeoCode = c.WorkAddressUbigeoCode,
            WorkAddress = c.WorkAddress,
            ContactName = c.ContactName,
            ContactPhone = c.ContactPhone,
            ContactEmail = c.ContactEmail,
            Reference = c.Reference,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            DispatchDate = c.DispatchDate,
            FleteCode = c.FleteCode,
            IgvRate = c.IgvRate,
            ProcessDate = c.ProcessDate,
            GrossAmount = c.GrossAmount,
            IgvAmount = c.IgvAmount,
            NetAmount = c.NetAmount,
            Status = c.Status.ToString(),
            OriginNegocioCode = c.OriginNegocioCode,
            OriginYear = c.OriginYear,
            OriginMonth = c.OriginMonth,
            OriginCode = c.OriginCode,
            ApprovedBy = c.ApprovedBy,
            ApprovedAt = c.ApprovedAt,
            CancelReason = c.CancelReason,
            CanceledBy = c.CanceledBy,
            CanceledAt = c.CanceledAt,
            IsPrinted = c.IsPrinted,
            Observations = c.Observations,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            RowVersion = c.RowVersion
        };
    }
}
