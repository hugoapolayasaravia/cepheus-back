using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.UpdateRegimenLaboral
{
    public record UpdateRegimenLaboralCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<RegimenLaboralResponse>;
}