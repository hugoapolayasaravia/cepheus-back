using Cepheus.Application.Features.Logistica.Maestros.Conductores.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Conductores.UpdateConductor
{
    public record UpdateConductorCommand(
        string Code,
        string DocumentTypeCode,
        string DocumentNumber,
        string FirstName,
        string LastName,
        string DriverLicenseNumber,
        string? LicenseCategory,
        string? Phone,
        string? Email,
        string? Observations,
        byte[] RowVersion
    ) : IRequest<ConductorResponse>;
}
