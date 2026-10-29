using Cepheus.Application.Features.Logistica.Maestros.Transportistas.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Transportistas.UpdateTransportista
{
    public record UpdateTransportistaCommand(
        string Code,
        string DocumentTypeCode,
        string DocumentNumber,
        string LegalName,
        string? TradeName,
        string? Address,
        string? UbigeoCode,
        string? Phone,
        string? Email,
        string? MtcRegistrationNumber,
        string? CertificationNumber,
        bool IsOwnFleet,
        string? Observations,
        byte[] RowVersion
    ) : IRequest<TransportistaResponse>;
}
