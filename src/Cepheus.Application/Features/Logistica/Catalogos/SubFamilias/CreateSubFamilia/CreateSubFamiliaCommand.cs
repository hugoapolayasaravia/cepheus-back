using Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.CreateSubFamilia
{
    public record CreateSubFamiliaCommand(
        string FamiliaCode,
        string Name
    ) : IRequest<SubFamiliaResponse>;
}