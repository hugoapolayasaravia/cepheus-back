using Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.CreateNivelEducativo
{
    public record CreateNivelEducativoCommand(
        string Name
    ) : IRequest<NivelEducativoResponse>;
}