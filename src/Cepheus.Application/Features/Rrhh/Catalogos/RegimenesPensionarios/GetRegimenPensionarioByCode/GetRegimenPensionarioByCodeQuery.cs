using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.GetRegimenPensionarioByCode
{
    public record GetRegimenPensionarioByCodeQuery(string Code) : IRequest<RegimenPensionarioResponse>;
}