using Cepheus.Application.Features.Logistica.Maestros.Conductores.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Conductores.CreateConductor
{
    public record CreateConductorCommand(
        string DocumentTypeCode,
        string DocumentNumber,
        string FirstName,
        string LastName,
        string DriverLicenseNumber,
        string? LicenseCategory,
        string? Phone,
        string? Email,
        string? Observations
    ) : IRequest<ConductorResponse>;
}
