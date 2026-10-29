using Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.CreateNivelTrabajador
{
    public record CreateNivelTrabajadorCommand(
        string Name
    ) : IRequest<NivelTrabajadorResponse>;
}