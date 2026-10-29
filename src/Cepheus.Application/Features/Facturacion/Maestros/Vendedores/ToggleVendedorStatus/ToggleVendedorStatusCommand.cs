using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vendedores.ToggleVendedorStatus
{
    public record ToggleVendedorStatusCommand(string Code) : IRequest<bool>;
}
