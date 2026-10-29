using Cepheus.Application.Features.Facturacion.Maestros.Choferes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Choferes.CreateChofer
{
    public record CreateChoferCommand(
        string TransportistaCode,
        string FullName,
        string DriverLicenseNumber,
        string? Observations
    ) : IRequest<ChoferResponse>;
}
