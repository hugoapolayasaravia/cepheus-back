using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Transportistas.CreateTransportista
{
    public record CreateTransportistaCommand(
        string DocumentTypeCode,
        string DocumentNumber,
        string Name,
        string? Address,
        string? UbigeoCode,
        string? Phone,
        string? Email,
        string? MtcInternalCode
    ) : IRequest<TransportistaResponse>;
}
