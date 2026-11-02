using Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.UpdateClienteResponsableLetra
{
    public record UpdateClienteResponsableLetraCommand(
        string ClienteCode, string Sequence, string? PaternalSurname, string? MaternalSurname, string? FirstNames,
        string? DocumentTypeCode, string? DocumentNumber, string? Address, string? Phone)
        : IRequest<ClienteResponsableLetraResponse>, IClienteResponsableLetraInput;
}
