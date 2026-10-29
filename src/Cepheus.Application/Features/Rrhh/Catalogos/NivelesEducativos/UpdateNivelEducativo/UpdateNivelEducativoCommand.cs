using Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.UpdateNivelEducativo
{
    public record UpdateNivelEducativoCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<NivelEducativoResponse>;
}