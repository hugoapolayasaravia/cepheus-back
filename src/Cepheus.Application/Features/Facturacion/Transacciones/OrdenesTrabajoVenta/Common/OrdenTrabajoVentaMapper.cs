using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;

public static class OrdenTrabajoVentaMapper
{
    public static OrdenTrabajoVentaResponse Map(OrdenTrabajoVenta ot, IReadOnlyCollection<HojaCorte> hojas, decimal igvRate)
    {
        var hojasVigentes = hojas.Any(h => h.Status != EstadoHojaCorte.Anulado);

        return new OrdenTrabajoVentaResponse
        {
            Code = ot.Code,
            Type = ot.Type,
            Stage = ot.Stage,
            Status = ot.Status,
            StatusDescription = ot.Status.ToString(),

            ClienteCode = ot.ClienteCode,
            ClientName = ot.Cliente.Name ?? string.Empty,
            ObraCode = ot.ObraCode,
            WorkName = ot.Obra.Description,
            WorkAddress = string.IsNullOrWhiteSpace(ot.Obra.DeliveryAddress) ? ot.Obra.Address : ot.Obra.DeliveryAddress!,
            ResponsibleName = ot.Obra.ResponsibleName,
            ResponsiblePhone = ot.Obra.ResponsiblePhone,

            DispatchDate = ot.DispatchDate,
            DispatchTime = ot.DispatchTime,

            VendedorCode = ot.VendedorCode,
            VendedorName = ot.Vendedor.Name,
            CurrencyCode = ot.CurrencyCode,
            MotivoCode = ot.MotivoCode,
            MotivoName = ot.Motivo.Name,

            Observations = ot.Observations,
            CreditObservations = ot.CreditObservations,
            CutSheetObservations = ot.CutSheetObservations,

            Total = ot.Total,
            IgvRate = igvRate,
            TotalWithIgv = OrdenTrabajoVentaRules.WithIgv(ot.Total, igvRate),
            ProductTypes = OrdenTrabajoVentaRules.ComputeProductTypes(ot),

            OriginCode = ot.OriginCode,
            CotizacionNegocioCode = ot.CotizacionNegocioCode,
            CotizacionYear = ot.CotizacionYear,
            CotizacionMonth = ot.CotizacionMonth,
            CotizacionCode = ot.CotizacionCode,

            PurchaseOrderNumber = ot.PurchaseOrderNumber,
            IsModifiable = ot.IsModifiable,
            SentToApprovalAt = ot.SentToApprovalAt,
            ApprovedBy = ot.ApprovedBy,
            ApprovedAt = ot.ApprovedAt,
            ApprovalObservations = ot.ApprovalObservations,
            ApprovalDelayReason = ot.ApprovalDelayReason,
            PassedToProduction = ot.PassedToProduction,
            ProductionPassDate = ot.ProductionPassDate,
            ProductionPassReason = ot.ProductionPassReason,
            SpecialApprovalGranted = ot.SpecialApprovalGranted,
            SpecialApprovalBy = ot.SpecialApprovalBy,
            SpecialApprovalAt = ot.SpecialApprovalAt,
            SpecialApprovalObservations = ot.SpecialApprovalObservations,

            CanceledBy = ot.CanceledBy,
            CanceledAt = ot.CanceledAt,
            CancelReason = ot.CancelReason,
            InactivatedBy = ot.InactivatedBy,
            InactivatedAt = ot.InactivatedAt,
            InactivationReason = ot.InactivationReason,

            CreatedAt = ot.CreatedAt,
            CreatedBy = ot.CreatedBy,

            Detalles = ot.Detalles
                .OrderBy(d => d.Item)
                .Select(d => MapDetalle(d, hojasVigentes))
                .ToList(),

            HojasCorte = hojas.Select(h => new OrdenTrabajoVentaHojaCorteResponse
            {
                Year = h.Year,
                Code = h.Code,
                Status = h.Status,
                Sector = h.Sector,
                Floor = h.Floor
            }).ToList(),

            Aprobacion = ot.Aprobacion is null ? null : MapAprobacion(ot.Aprobacion)
        };
    }

    public static OrdenTrabajoVentaDetalleResponse MapDetalle(OrdenTrabajoVentaDetalle d, bool hasHojasCorte) => new()
    {
        Item = d.Item,
        ProductoTipoCode = d.ProductoTipoCode,
        ProductoCode = d.ProductoCode,
        ProductoName = d.Producto.Name,
        UnitCode = d.Producto.UnitCode,
        UnitName = d.Producto.Unit.Name,
        NegocioCode = d.NegocioCode,
        Quantity = d.Quantity,
        ApprovedQuantity = d.ApprovedQuantity,
        ProgrammedQuantity = d.ProgrammedQuantity,
        DeliveredQuantity = d.DeliveredQuantity,
        UnitPrice = d.UnitPrice,
        Total = d.Total,
        Status = d.Status,
        IsLockedByHojaCorte = hasHojasCorte && OrdenTrabajoVentaRules.IsHojaCorteProducto(d.ProductoTipoCode)
    };

    public static OrdenTrabajoVentaAprobacionResponse MapAprobacion(OrdenTrabajoVentaAprobacion a) => new()
    {
        CreditsObservations = a.CreditsObservations,
        Document1Number = a.Document1Number,
        DocumentType = a.DocumentType,
        ApprovedQuantity = a.ApprovedQuantity,
        ApprovedAt = a.ApprovedAt,
        PaymentType = a.PaymentType,
        CreditStatus = a.CreditStatus,
        DocumentFlow = a.DocumentFlow,
        ValorizationCode = a.ValorizationCode,
        ValorizationWeekday = a.ValorizationWeekday,
        ValorizationDays = a.ValorizationDays,
        Document2Number = a.Document2Number,
        HasTwoDocuments = a.HasTwoDocuments,
        Plant1Code = a.Plant1Code,
        Plant1Name = a.Plant1?.Name,
        Plant2Code = a.Plant2Code,
        Plant2Name = a.Plant2?.Name,
        GuideMotivoCode = a.GuideMotivoCode,
        PaymentMethodCode = a.PaymentMethodCode,
        BankCode = a.BankCode,
        BankName = a.Bank?.Name,
        PaymentDocumentNumber = a.PaymentDocumentNumber,
        FormaPagoVentaCode = a.FormaPagoVentaCode,
        FormaPagoVentaName = a.FormaPagoVenta.Name
    };

    public static OrdenTrabajoVentaListadoResponse MapListado(OrdenTrabajoVenta ot, decimal igvRate, bool obraIsCredit)
    {
        var paymentLetter = ot.Type switch
        {
            TipoOrdenTrabajoVenta.Anticipo => "A",
            TipoOrdenTrabajoVenta.Descuento => "D",
            _ => obraIsCredit ? "R" : "C"
        };

        return new OrdenTrabajoVentaListadoResponse
        {
            Code = ot.Code,
            Date = ot.CreatedAt.Date,
            Time = ot.DispatchTime ?? string.Empty,
            ClienteCode = ot.ClienteCode,
            ClientName = ot.Cliente.Name ?? string.Empty,
            ObraCode = ot.ObraCode,
            VendedorAbbreviation = ot.Vendedor.Abbreviation ?? ot.Vendedor.Name,
            Total = ot.Total,
            TotalWithIgv = OrdenTrabajoVentaRules.WithIgv(ot.Total, igvRate),
            CurrencyCode = ot.CurrencyCode,
            Type = ot.Type,
            Stage = ot.Stage,
            Status = ot.Status,
            StatusDescription = ot.Status == EstadoOrdenTrabajoVenta.Cerrado ? "Atendido 100%" : ot.Status.ToString(),
            StatusLetter = OrdenTrabajoVentaRules.StatusLetter(ot.Status),
            TypeLetter = OrdenTrabajoVentaRules.TypeLetter(ot.Type, ot.OriginCode),
            PaymentLetter = paymentLetter,
            ProductTypes = OrdenTrabajoVentaRules.ComputeProductTypes(ot),
            OriginCode = ot.OriginCode,
            ApprovedBy = ot.ApprovedBy,
            ApprovedAt = ot.ApprovedAt,
            ApprovalObservations = ot.ApprovalObservations,
            IsModifiable = ot.IsModifiable,
            PassedToProduction = ot.PassedToProduction,
            CreatedBy = ot.CreatedBy
        };
    }
}
