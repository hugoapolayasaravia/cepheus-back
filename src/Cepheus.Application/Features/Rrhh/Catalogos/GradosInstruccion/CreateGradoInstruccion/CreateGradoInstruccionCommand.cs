using Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.CreateGradoInstruccion
{
    public record CreateGradoInstruccionCommand(
        string Name
    ) : IRequest<GradoInstruccionResponse>;
}