using Cepheus.Application.Features.Rrhh.Catalogos.Horarios.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Horarios.GetHorarioByCode
{
    public record GetHorarioByCodeQuery(string Code) : IRequest<HorarioResponse>;
}