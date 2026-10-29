using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vendedores.CreateVendedor
{
    public record CreateVendedorCommand(
        string Name,
        string? Abbreviation,
        string? Address,
        string? Phone,
        string? Email,
        string? Title,
        int? UserId
    ) : IRequest<VendedorResponse>;
}
