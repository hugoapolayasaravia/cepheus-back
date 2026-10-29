using Cepheus.Application.Features.Rrhh.Catalogos.Alergias.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Alergias.UpdateAlergia
{
    public record UpdateAlergiaCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<AlergiaResponse>;
}