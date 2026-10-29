using Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.UpdateGradoInstruccion
{
    public record UpdateGradoInstruccionCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<GradoInstruccionResponse>;
}