using Cepheus.Application.Features.Rrhh.Catalogos.Horarios.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Horarios.UpdateHorario
{
    public record UpdateHorarioCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<HorarioResponse>;
}