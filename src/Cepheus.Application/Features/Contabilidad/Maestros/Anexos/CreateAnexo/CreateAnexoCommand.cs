using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.Anexos.CreateAnexo
{
    public record CreateAnexoCommand(
        string TipoAnexoCode,
        string? Name,
        string? LinkAccount1Code,
        string? LinkAccount2Code,
        string? Flag
    ) : IRequest<AnexoResponse>;
}
