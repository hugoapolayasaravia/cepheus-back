using Cepheus.Application.Features.Logistica.Catalogos.Familias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Familias.CreateFamilia
{
    public record CreateFamiliaCommand(
        string Code,
        string Name
    ) : IRequest<FamiliaResponse>;
}