using Cepheus.Application.Features.Logistica.Maestros.Proveedores.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Proveedores.CreateProveedor
{
    public record CreateProveedorCommand(
        string DocumentTypeCode,
        string DocumentNumber,
        string LegalName,
        string? TradeName,
        ProviderType ProviderType,
        ProviderOrigin Origin,
        SunatCondition? SunatCondition,
        SunatTaxpayerStatus? SunatStatus,
        string? Observations
    ) : IRequest<ProveedorResponse>;
}
