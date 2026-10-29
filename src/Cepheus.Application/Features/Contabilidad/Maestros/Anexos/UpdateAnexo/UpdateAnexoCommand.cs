using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.Anexos.UpdateAnexo
{
    public record UpdateAnexoCommand(
        string TipoAnexoCode,
        string Code,
        string? Name,
        string? LinkAccount1Code,
        string? LinkAccount2Code,
        string? Flag,
        byte[] RowVersion
    ) : IRequest<AnexoResponse>;
}
