using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vendedores.GetVendedorByCode
{
    public record GetVendedorByCodeQuery(string Code) : IRequest<VendedorResponse>;
}
