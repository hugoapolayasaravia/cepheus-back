using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vendedores.UpdateVendedor
{
    public record UpdateVendedorCommand(
        string Code,
        string Name,
        string? Abbreviation,
        string? Address,
        string? Phone,
        string? Email,
        string? Title,
        int? UserId,
        byte[] RowVersion
    ) : IRequest<VendedorResponse>;
}
