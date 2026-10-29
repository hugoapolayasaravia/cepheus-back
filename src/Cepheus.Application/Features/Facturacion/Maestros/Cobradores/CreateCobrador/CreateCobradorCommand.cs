using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Cobradores.CreateCobrador
{
    public record CreateCobradorCommand(
        string Name,
        string? Address,
        string? Phone,
        string? Email,
        int? UserId
    ) : IRequest<CobradorResponse>;
}
