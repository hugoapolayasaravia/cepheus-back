using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Transportistas.UpdateTransportista
{
    public record UpdateTransportistaCommand(
        string Code,
        string DocumentTypeCode,
        string DocumentNumber,
        string Name,
        string? Address,
        string? UbigeoCode,
        string? Phone,
        string? Email,
        string? MtcInternalCode,
        byte[] RowVersion
    ) : IRequest<TransportistaResponse>;
}
