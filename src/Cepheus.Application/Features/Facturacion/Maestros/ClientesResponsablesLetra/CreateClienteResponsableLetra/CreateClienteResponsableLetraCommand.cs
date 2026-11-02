using Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.CreateClienteResponsableLetra
{
    /// <summary>Alta de un aval del cliente. La secuencia (001, 002, ...) la asigna el sistema.</summary>
    public record CreateClienteResponsableLetraCommand(
        string ClienteCode, string? PaternalSurname, string? MaternalSurname, string? FirstNames,
        string? DocumentTypeCode, string? DocumentNumber, string? Address, string? Phone)
        : IRequest<ClienteResponsableLetraResponse>, IClienteResponsableLetraInput;
}
