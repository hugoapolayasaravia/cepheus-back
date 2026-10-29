using Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.CreateEstadoCivil
{
    public record CreateEstadoCivilCommand(
        string Name
    ) : IRequest<EstadoCivilResponse>;
}