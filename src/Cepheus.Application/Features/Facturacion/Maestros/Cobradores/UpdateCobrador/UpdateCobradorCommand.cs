using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Cobradores.UpdateCobrador
{
    public record UpdateCobradorCommand(
        string Code,
        string Name,
        string? Address,
        string? Phone,
        string? Email,
        int? UserId,
        byte[] RowVersion
    ) : IRequest<CobradorResponse>;
}
