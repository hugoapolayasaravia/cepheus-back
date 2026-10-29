using Cepheus.Application.Features.Rrhh.Catalogos.Sexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Sexos.CreateSexo
{
    public record CreateSexoCommand(
        string Name
    ) : IRequest<SexoResponse>;
}