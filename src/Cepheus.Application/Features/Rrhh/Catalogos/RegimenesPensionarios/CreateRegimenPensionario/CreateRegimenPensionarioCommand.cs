using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.CreateRegimenPensionario
{
    public record CreateRegimenPensionarioCommand(
        string Name
    ) : IRequest<RegimenPensionarioResponse>;
}