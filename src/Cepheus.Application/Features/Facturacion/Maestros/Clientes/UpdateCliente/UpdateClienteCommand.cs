using Cepheus.Application.Features.Facturacion.Maestros.Clientes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Clientes.UpdateCliente
{
    public record UpdateClienteCommand(
        string Code,
        Cepheus.Domain.Facturacion.Enum.TipoPersona PersonType,
        string DocumentTypeCode,
        string DocumentNumber,
        string? Name,
        string Address,
        string UbigeoCode,
        string Phone,
        string? ParentClientCode,
        string TipoClienteCode,
        string ClasificacionClienteCode,
        string? LegalRepresentativeName,
        string? LegalRepresentativePhone,
        string? LegalRepresentativeDni,
        string ContactName,
        string ContactPhone,
        string ContactEmail,
        string? Observations,
        bool IsVip,
        bool RequiresCashOnly,
        bool HasGlobalCreditLine,
        decimal? GlobalCreditAmount,
        bool? RequiresPurchaseOrderApproval,
        bool? RequiresWorkOrderApproval,
        string FormaPagoVentaCode,
        string? CurrencyCode,
        bool? RequiresManagementApproval,
        byte[] RowVersion
    ) : IRequest<ClienteResponse>;
}
