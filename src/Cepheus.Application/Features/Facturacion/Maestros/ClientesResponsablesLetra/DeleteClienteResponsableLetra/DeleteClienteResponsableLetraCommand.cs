using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.DeleteClienteResponsableLetra
{
    public record DeleteClienteResponsableLetraCommand(string ClienteCode, string Sequence) : IRequest<Unit>;
}
