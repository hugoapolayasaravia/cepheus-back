using Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.GetClientesResponsablesLetra
{
    /// <summary>Responsables / avales de un cliente (legacy dw_bus_cre_avales).</summary>
    public record GetClientesResponsablesLetraQuery(string ClienteCode) : IRequest<List<ClienteResponsableLetraResponse>>;
}
