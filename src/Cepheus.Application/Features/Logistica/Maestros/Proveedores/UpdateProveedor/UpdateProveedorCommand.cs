using Cepheus.Application.Features.Logistica.Maestros.Proveedores.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Proveedores.UpdateProveedor
{
    public record UpdateProveedorCommand(
        string Code,
        string DocumentTypeCode,
        string DocumentNumber,
        string LegalName,
        string? TradeName,
        ProviderType ProviderType,
        ProviderOrigin Origin,
        SunatCondition? SunatCondition,
        SunatTaxpayerStatus? SunatStatus,
        string? Observations,
        byte[] RowVersion
    ) : IRequest<ProveedorResponse>;
}
