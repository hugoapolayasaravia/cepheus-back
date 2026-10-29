using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.UpdateRegimenPensionario
{
    public record UpdateRegimenPensionarioCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<RegimenPensionarioResponse>;
}