using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.GetRegimenLaboralByCode
{
    public record GetRegimenLaboralByCodeQuery(string Code) : IRequest<RegimenLaboralResponse>;
}