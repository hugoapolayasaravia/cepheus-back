using Cepheus.Application.Features.Rrhh.Catalogos.Alergias.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Alergias.CreateAlergia
{
    public record CreateAlergiaCommand(
        string Name
    ) : IRequest<AlergiaResponse>;
}