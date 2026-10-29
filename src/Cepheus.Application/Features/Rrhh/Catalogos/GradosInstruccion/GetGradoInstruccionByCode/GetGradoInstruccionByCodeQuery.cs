using Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.GetGradoInstruccionByCode
{
    public record GetGradoInstruccionByCodeQuery(string Code) : IRequest<GradoInstruccionResponse>;
}