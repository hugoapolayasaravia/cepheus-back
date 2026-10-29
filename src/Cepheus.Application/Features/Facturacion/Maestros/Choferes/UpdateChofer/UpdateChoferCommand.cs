using Cepheus.Application.Features.Facturacion.Maestros.Choferes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Choferes.UpdateChofer
{
    public record UpdateChoferCommand(
        string TransportistaCode,
        string Code,
        string FullName,
        string DriverLicenseNumber,
        string? Observations,
        byte[] RowVersion
    ) : IRequest<ChoferResponse>;
}
