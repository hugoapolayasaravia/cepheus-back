using Cepheus.Application.Features.Facturacion.Maestros.Obras.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Obras.UpdateObra
{
    public record UpdateObraCommand(
        string ClienteCode,
        string Code,
        string Description,
        string Address,
        string UbigeoCode,
        string? Observations,
        string? DeliveryAddress,
        string DeliveryUbigeoCode,
        string BillingAddress,
        string BillingUbigeoCode,
        string? ResponsibleName,
        string ResponsiblePhone,
        string ResponsibleEmail,
        string? FormaPagoVentaCode,
        string CobradorCode,
        string VendedorCode,
        string? AnalisisVentaCode,
        decimal CreditLimit,
        string CreditCurrencyCode,
        DateTime? EntryDate,
        bool HasSurcharge,
        string ShortName,
        string? TipoValorizacionCode,
        bool? RequiresValorizacion,
        string? ScheduledWeekday,
        bool IsProject,
        bool RequiresPrinting,
        bool? RequiresManagementApproval,
        byte[] RowVersion
    ) : IRequest<ObraResponse>;
}
