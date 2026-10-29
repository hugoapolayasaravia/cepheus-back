using Cepheus.Application.Features.Rrhh.Catalogos.Sexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Sexos.UpdateSexo
{
    public record UpdateSexoCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<SexoResponse>;
}